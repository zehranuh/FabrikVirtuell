using System;
using System.Threading.Tasks;

namespace Businesslogic
{
    class Program
    {
        static async Task Main(string[] args)
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
                        AddJob(jobManager);
                        break;
                    case "startjobs":
                        await jobManager.StartJobsAsync(machine);
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

        private static void AddJob(JobManager jobManager)
        {
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
        }
    }
}
