using Businesslogic;

namespace Businesslogic
{
    public class Job
    {
        public enum State
        {
            Pending,
            InWork,
            Done,
            Stopped
        }

        public int JobId { get; set; }
        public string JobName { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public int ProducedQuantity { get; set; }
        public State CurrentState { get; set; }
        public int MachineID { get; set; }

        private SignalLight signalLight = new SignalLight();

        public Job(int jobId, string jobName, string product, int quantity, int producedQuantity, State currentState, int machineID)
        {
            JobId = jobId;
            JobName = jobName;
            Product = product;
            Quantity = quantity;
            ProducedQuantity = producedQuantity;
            CurrentState = currentState;
            MachineID = machineID;
        }

        public Job(string jobName, string product, int quantity)
        {
            JobName = jobName;
            Product = product;
            Quantity = quantity;
            ProducedQuantity = 0;
            CurrentState = State.Pending;
        }

        public void Start()
        {
            if (CurrentState == State.Pending || CurrentState == State.Stopped)
            {
                CurrentState = State.InWork;
            }
        }

        public void Stop()
        {
            if (CurrentState == State.InWork)
            {
                CurrentState = State.Stopped;
            }
        }

        public void Complete()
        {
            if (CurrentState == State.InWork)
            {
                CurrentState = State.Done;
            }
        }
    }
}