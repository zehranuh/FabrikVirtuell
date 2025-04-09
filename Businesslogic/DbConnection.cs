using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Businesslogic
{
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
                        Job.State state = (Job.State)Enum.Parse(typeof(Job.State), reader["CurrentState"].ToString());
                        Job job = new Job(
                            Convert.ToInt32(reader["JobId"]),
                            reader["JobName"].ToString(),
                            reader["Product"].ToString(),
                            Convert.ToInt32(reader["Quantity"]),
                            Convert.ToInt32(reader["ProducedQuantity"]),
                            state,
                            Convert.ToInt32(reader["MachineID"])
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
                string query = "INSERT INTO Jobs (JobName, Product, Quantity, ProducedQuantity, CurrentState, MachineID) " +
                               "VALUES (@JobName, @Product, @Quantity, @ProducedQuantity, @CurrentState, @MachineID)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@JobName", job.JobName);
                cmd.Parameters.AddWithValue("@Product", job.Product);
                cmd.Parameters.AddWithValue("@Quantity", job.Quantity);
                cmd.Parameters.AddWithValue("@ProducedQuantity", job.ProducedQuantity);
                cmd.Parameters.AddWithValue("@CurrentState", job.CurrentState.ToString()); 
                cmd.Parameters.AddWithValue("@MachineID", job.MachineID);
                cmd.ExecuteNonQuery();
            }
        }


        public void DeleteJob(int jobId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Jobs WHERE JobID = @JobID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@JobID", jobId);
                cmd.ExecuteNonQuery();
            }
        }

        public void MarkJobAsDone(int jobId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Jobs SET CurrentState = 'Done' WHERE JobId = @JobId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@JobId", jobId);
                cmd.ExecuteNonQuery();
            }
        }

    }
}