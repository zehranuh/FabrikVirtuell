namespace RestAPI.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }  
        public string CurrentState { get; set; }
        public int MachineID { get; set; }
    }

    public class Machine
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string SignalLightState { get; set; }
    }
}