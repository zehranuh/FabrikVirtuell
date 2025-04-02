using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Businesslogic
{
    public class Machine
    {
        private enum State { Ready, Running, Error }
        private State currentState = State.Ready;
        private SignalLight signalLight = new SignalLight();

        public event Action<string> MachineFailed;

        public Machine()
        {
            signalLight.SetState(SignalLight.State.Yellow);
        }

        public void Start()
        {
            if (currentState == State.Ready)
            {
                currentState = State.Running;
                signalLight.SetState(SignalLight.State.Green);
            }
        }

        public void Stop()
        {
            if (currentState == State.Running || currentState == State.Ready)
            {
                currentState = State.Ready;
                signalLight.SetState(SignalLight.State.Yellow);
            }
        }

        public void Fail()
        {
            if (currentState == State.Running)
            {
                currentState = State.Error;
                signalLight.SetState(SignalLight.State.Red);
                MachineFailed?.Invoke("Maschine ist im Error-Zustand");
            }
        }

        public bool FixError(string code)
        {
            if (currentState == State.Error && code == "9944")
            {
                currentState = State.Ready;
                signalLight.SetState(SignalLight.State.Green);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CheckRunning()
        {
            if (currentState == State.Running)
            {
                Console.WriteLine("Maschine arbeitet...");
            }
        }

        public bool IsRunning()
        {
            return currentState == State.Running;
        }

        public bool IsNotRunning()
        {
            return currentState == State.Ready;
        }

        public SignalLight.State GetSignalLightState()
        {
            return signalLight.GetState();
        }

        public string Status()
        {
            return $"{currentState}";
        }

        public string GetStatus()
        {
            return currentState.ToString();
        }
    }
}
