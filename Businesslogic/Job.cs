using Businesslogic;
    
    namespace Businesslogic
{
    public class Job
    {
        public enum State { Pending, InWork, Done, Stopped }
        public string JobName { get; }
        public string Product { get; }
        public int Quantity { get; }
        private int producedQuantity;
        public State CurrentState { get; private set; }
        private SignalLight signalLight = new SignalLight();

        public Job(string jobName, string product, int quantity)
        {
            JobName = jobName;
            Product = product;
            Quantity = quantity;
            producedQuantity = 0;
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
