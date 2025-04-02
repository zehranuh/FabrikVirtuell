using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Businesslogic
{
    class Program
    {
        static void Main(string[] args)
        {
            DbConnection dbConnection = new DbConnection();
            dbConnection.TestConnection();

            JobManager jobManager = new JobManager(dbConnection);
            Machine machine = new Machine();

            while (true)
            {
                Console.WriteLine("Befehle: addjob, startjobs, status, exit");
                string input = Console.ReadLine().ToLower();
                switch (input)
                {
                    case "addjob":
                        Console.WriteLine("Wähle einen Job: 1=Auto bauen, 2=Kabel machen, 3=Metall bearbeiten.");
                        int jobType = int.Parse(Console.ReadLine());
                        string product = jobType switch
                        {
                            1 => "Auto",
                            2 => "Kabel",
                            3 => "Metallstück",
                            _ => throw new ArgumentException("Ungültiger Job-Typ")
                        };
                        Console.WriteLine("Stückzahl:");
                        int quantity = int.Parse(Console.ReadLine());
                        Console.WriteLine("Jobname:");
                        string jobName = Console.ReadLine();
                        Job job = new Job(jobName, product, quantity);
                        jobManager.AddJob(job);
                        break;
                    case "startjobs":
                        jobManager.StartJobs(machine);
                        break;
                    case "status":
                        machine.Status();
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Ungültiger Befehl");
                        break;
                }
            }
        }
    }

    public class DbConnection
    {
        private string connectionString = "Server=localhost;Database=my_database;User ID=root;Password=Calcio7ronaldo;";

        public void TestConnection()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Verbindung erfolgreich hergestellt.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler bei der Verbindung: {ex.Message}");
                }
            }
        }

        public List<Job> GetJobs()
        {
            List<Job> jobs = new List<Job>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Jobs";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Job job = new Job(
                            reader["JobName"].ToString(),
                            reader["Product"].ToString(),
                            Convert.ToInt32(reader["Quantity"])
                        );
                        jobs.Add(job);
                    }
                }
            }

            return jobs;
        }

        public void AddJob(Job job)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Jobs (JobName, Product, Quantity, ProducedQuantity, CurrentState, MachineID) VALUES (@JobName, @Product, @Quantity, @ProducedQuantity, @CurrentState, @MachineID)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@JobName", job.JobName);
                cmd.Parameters.AddWithValue("@Product", job.Product);
                cmd.Parameters.AddWithValue("@Quantity", job.Quantity);
                cmd.Parameters.AddWithValue("@ProducedQuantity", 0);
                cmd.Parameters.AddWithValue("@CurrentState", job.CurrentState.ToString());
                cmd.Parameters.AddWithValue("@MachineID", 1);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class Job
    {
        public string JobName { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string CurrentState { get; set; }

        public Job(string jobName, string product, int quantity)
        {
            JobName = jobName;
            Product = product;
            Quantity = quantity;
            CurrentState = "Pending";
        }
    }

    public class JobManager
    {
        private DbConnection _dbConnection;

        public JobManager(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void AddJob(Job job)
        {
            _dbConnection.AddJob(job);
        }

        public void StartJobs(Machine machine)
        {
            List<Job> jobs = _dbConnection.GetJobs();
            foreach (var job in jobs)
            {
                machine.ProcessJob(job);
            }
        }
    }

    public class Machine
    {
        public void ProcessJob(Job job)
        {
            Console.WriteLine($"Processing job: {job.JobName}, Product: {job.Product}, Quantity: {job.Quantity}");
            job.CurrentState = "Completed";
        }

        public void Status()
        {
            Console.WriteLine("Machine status: All systems operational.");
        }
    }
}