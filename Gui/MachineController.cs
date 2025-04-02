using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Businesslogic
{
    public class MachineController
    {
        private Machine _machine;
        private JobManager _jobManager;
        private MainWindow _mainWindow;

        public event Action<string> JobStatusChanged;
        public event Action<string> JobCompleted;
        public event Action<string> MachineFailed;

        public MachineController(Machine machine, JobManager jobManager, MainWindow mainWindow)
        {
            _machine = machine;
            _jobManager = jobManager;
            _mainWindow = mainWindow;

            _jobManager.JobStatusChanged += UpdateJobStatus;
            _jobManager.JobCompleted += UpdateJobStatus;
            _jobManager.MachineFailed += ShowErrorMessage;
            _machine.MachineFailed += ShowErrorMessage;
        }

        private void UpdateJobStatus(string status)
        {
            JobStatusChanged?.Invoke(status);
        }

        private void ShowErrorMessage(string message)
        {
            MachineFailed?.Invoke(message);
        }

        public async void StartJobs()
        {
            if (_jobManager.GetJobs().Any())
            {
                var job = _jobManager.GetJobs().First();
                _mainWindow.JobStatusLabel.Content = $"Starte Job: {job.JobName}";
                bool result = await _jobManager.StartJobsAsync(_machine);
                if (!result)
                {
                    if (_machine.GetStatus() == "Error")
                    {
                        MessageBox.Show($"Fehler! Die Maschine ist im Error-Zustand. Geben Sie '9944' ein, um den Fehler zu beheben.");
                        string errorCode = Microsoft.VisualBasic.Interaction.InputBox("Fehlercode eingeben:", "Fehlerbehebung", "");
                        while (errorCode != "9944")
                        {
                            MessageBox.Show("Falscher Code. Fehler nicht behoben.");
                            errorCode = Microsoft.VisualBasic.Interaction.InputBox("Fehlercode eingeben:", "Fehlerbehebung", "");
                        }
                        if (_machine.FixError(errorCode))
                        {
                            MessageBox.Show("Fehler behoben! Füge deinen Job nochmals hinzu.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Keine weiteren Jobs in der Wartenschlange.");
                    }
                }
                else
                {
                    MessageBox.Show("Alle Produkte sind produziert.");
                }
                _mainWindow.JobStatusLabel.Content = "Keine Jobs in Bearbeitung";
            }
            else
            {
                MessageBox.Show("Keine Jobs in der Warteschlange.");
            }
            _mainWindow.UpdateJobList();
        }

        public void StopCurrentJob()
        {
            _jobManager.StopCurrentJob();
        }

        public void ContinueJob()
        {
            _jobManager.ContinueJob(_machine);
        }

        public void StartMachine()
        {
            _machine.Start();
            UpdateJobStatus("Maschine gestartet.");
        }

        public void StopMachine()
        {
            _machine.Stop();
            UpdateJobStatus("Maschine gestoppt.");
        }

        public void FailMachine()
        {
            _machine.Fail();
            UpdateJobStatus("Maschine im Error-Zustand.");
        }

        public void FixMachineError(string code)
        {
            if (_machine.FixError(code))
            {
                _jobManager.ContinueJob(_machine);
                JobStatusChanged?.Invoke("Fehler behoben! Füge nochmals deinen Job hinzu.");
            }
            else
            {
                JobStatusChanged?.Invoke("Falscher Code. Fehler nicht behoben.");
            }
        }

        public string GetMachineStatus()
        {
            return _machine.Status();
        }

        public bool IsMachineRunning()
        {
            return _machine.IsRunning();
        }

        public bool IsMachineNotRunning()
        {
            return _machine.IsNotRunning();
        }

        public SignalLight.State GetSignalLightState()
        {
            return _machine.GetSignalLightState();
        }

        public void AddJob(Job job)
        {
            _jobManager.AddJob(job);
            UpdateJobStatus($"Job {job.JobName} hinzugefügt.");
        }

        public void GetJobStatus()
        {
            _jobManager.Status();
        }

        public List<Job> GetJobs()
        {
            return _jobManager.GetJobs();
        }
    }
}