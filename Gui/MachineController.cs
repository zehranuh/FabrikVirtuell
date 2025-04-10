using Businesslogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace YourWpfAppNamespace
{
    public class MachineController
    {
        private Machine _machine;
        private JobManager _jobManager;
        private MainWindow _mainWindow;
        private DbConnection _dbConnection;

        public event Action<string> JobStatusChanged;
        public event Action<string> JobCompleted;
        public event Action<string> MachineFailed;

        public MachineController(Machine machine, JobManager jobManager, MainWindow mainWindow, DbConnection dbConnection)
        {
            _machine = machine;
            _jobManager = jobManager;
            _mainWindow = mainWindow;
            _dbConnection = dbConnection;

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
            var pendingJobs = _jobManager.GetPendingJobs();
            if (pendingJobs.Any())
            {
                var job = pendingJobs.First();
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
                            UpdateStatus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Keine weiteren Jobs in der Wartenschlange.");
                    }
                }
                else
                {
                    _dbConnection.MarkJobAsDone(job.JobId);
                    MessageBox.Show("Alle Produkte sind produziert.");
                    UpdateStatus();
                }
                _mainWindow.JobStatusLabel.Content = "Keine Jobs in Bearbeitung";
            }
            else
            {
                MessageBox.Show("Keine Jobs in der Warteschlange.");
            }
            _mainWindow.UpdateJobList();
        }

        public void RemoveJob(Job job)
        {
            _jobManager.RemoveJob(job);
            _mainWindow.UpdateJobList();
        }

        public void StopCurrentJob()
        {
            _jobManager.StopCurrentJob();
            UpdateStatus();
        }

        public void StartMachine()
        {
            _machine.Start();
            UpdateJobStatus("Maschine gestartet.");
            UpdateStatus();
        }

        public void StopMachine()
        {
            _machine.Stop();
            UpdateJobStatus("Maschine gestoppt.");
            UpdateStatus();
        }

        public void FailMachine()
        {
            _machine.Fail();
            UpdateJobStatus("Maschine im Error-Zustand.");
            UpdateStatus();
        }

        public void FixMachineError(string code)
        {
            if (_machine.FixError(code))
            {
                JobStatusChanged?.Invoke("Fehler behoben! Füge nochmals deinen Job hinzu.");
                _mainWindow.UpdateStatus();
            }
            else
            {
                JobStatusChanged?.Invoke("Falscher Code. Fehler nicht behoben.");
            }
            UpdateStatus();
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
            _mainWindow.UpdateJobList();
        }

        public List<Job> GetPendingJobs()
        {
            return _jobManager.GetPendingJobs();
        }

        private void UpdateStatus()
        {
            _mainWindow.StatusLabel.Content = GetMachineStatus();
            SignalLight.State signalLightState = GetSignalLightState();
            switch (signalLightState)
            {
                case SignalLight.State.Green:
                    _mainWindow.SignalLightEllipse.Fill = System.Windows.Media.Brushes.Green;
                    break;
                case SignalLight.State.Yellow:
                    _mainWindow.SignalLightEllipse.Fill = System.Windows.Media.Brushes.Yellow;
                    break;
                case SignalLight.State.Red:
                    _mainWindow.SignalLightEllipse.Fill = System.Windows.Media.Brushes.Red;
                    break;
                default:
                    _mainWindow.SignalLightEllipse.Fill = System.Windows.Media.Brushes.Gray;
                    break;
            }
        }
    }
}
