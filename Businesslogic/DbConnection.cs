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
}