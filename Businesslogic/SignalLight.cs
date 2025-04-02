using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Businesslogic
{
    public class SignalLight
    {
        public enum State { Green, Yellow, Red }
        private State currentState = State.Yellow;

        public void SetState(State newState)
        {
            currentState = newState;
            Console.WriteLine($"Signalleuchte: {currentState}");
        }

        public State GetState()
        {
            return currentState;
        }
    }
}
