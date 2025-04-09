using Businesslogic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YourWpfAppNamespace;

namespace Businesslogic
{
    public partial class MainWindow : Window
    {
        private MachineController _machineController;
        private DbConnection _databaseHelper;

        public ObservableCollection<Job> Jobs { get; set; } = new ObservableCollection<Job>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _databaseHelper = new DbConnection();
            Businesslogic.Machine machine = new Businesslogic.Machine();
            JobManager jobManager = new JobManager(_databaseHelper);
            _machineController = new MachineController(machine, jobManager, this, _databaseHelper);

            _machineController.JobStatusChanged += UpdateJobStatus;
            _machineController.JobCompleted += UpdateJobStatus;
            _machineController.MachineFailed += ShowErrorMessage;

            SignalLightEllipse.Fill = Brushes.Yellow;
            StatusLabel.Content = "Ready";
            Jobs.Add(new Job("test", "product", 10));
            LoadJobsFromDatabase();
        }

        private void LoadJobsFromDatabase()
        {
            var jobs = _databaseHelper.GetJobs();
            JobsListBox.Items.Clear();
            foreach (var job in jobs)
            {
                JobsListBox.Items.Add($"{job.JobName}: {job.Quantity} {job.Product}");
            }
            JobsLabel.Content = $"Anzahl der Jobs: {jobs.Count}";
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _machineController.StartMachine();
            UpdateStatus();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _machineController.StopMachine();
            UpdateStatus();
        }

        private void AddJobButton_Click(object sender, RoutedEventArgs e)
        {
            string jobName = JobNameTextBox.Text;
            int quantity;
            if (!int.TryParse(QuantityTextBox.Text, out quantity))
            {
                MessageBox.Show("Bitte geben Sie eine gültige Stückzahl ein.");
                return;
            }

            int machineID;
            if (!int.TryParse(MachineIDTextBox.Text, out machineID))
            {
                MessageBox.Show("Bitte geben Sie eine gültige MachineID ein.");
                return;
            }

            string product = "";
            if (JobTypeComboBox.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)JobTypeComboBox.SelectedItem;
                switch (selectedItem.Content.ToString().Split(' ')[0])
                {
                    case "1":
                        product = "Auto";
                        break;
                    case "2":
                        product = "Kabel";
                        break;
                    case "3":
                        product = "Metallstück";
                        break;
                    default:
                        MessageBox.Show("Bitte wählen Sie einen Produkt.");
                        return;
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Produkt.");
                return;
            }

            Job job = new Job(jobName, product, quantity)
            {
                MachineID = machineID
            };
            _machineController.AddJob(job);
            MessageBox.Show($"Job {jobName} hinzugefügt: {quantity} {product}");
            UpdateJobList();
        }

        private void StartJobsButton_Click(object sender, RoutedEventArgs e)
        {
            _machineController.StartJobs();
            UpdateStatus();
            LoadJobsFromDatabase();
            UpdateJobList();
        }

        private void StopCurrentJobButton_Click(object sender, RoutedEventArgs e)
        {
            _machineController.StopCurrentJob();
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            StatusLabel.Content = _machineController.GetMachineStatus();
            SignalLight.State signalLightState = _machineController.GetSignalLightState();
            switch (signalLightState)
            {
                case SignalLight.State.Green:
                    SignalLightEllipse.Fill = Brushes.Green;
                    break;
                case SignalLight.State.Yellow:
                    SignalLightEllipse.Fill = Brushes.Yellow;
                    break;
                case SignalLight.State.Red:
                    SignalLightEllipse.Fill = Brushes.Red;
                    break;
                default:
                    SignalLightEllipse.Fill = Brushes.Gray;
                    break;
            }
            UpdateJobList();
        }

        public void UpdateJobList()
        {
            JobsListBox.Items.Clear();
            var jobs = _machineController.GetPendingJobs();
            foreach (var job in jobs)
            {
                JobsListBox.Items.Add($"{job.JobName}: {job.Quantity} {job.Product}");
            }
            JobsLabel.Content = $"Anzahl der Jobs: {jobs.Count}";
        }

        private void UpdateJobStatus(string status)
        {
            Dispatcher.Invoke(() => JobStatusLabel.Content = status);
        }

        private void DeleteJobButton_Click(object sender, RoutedEventArgs e)
        {
            if (JobsListBox.SelectedItem != null)
            {
                string selectedJob = JobsListBox.SelectedItem.ToString();
                string jobName = selectedJob.Split(':')[0];

                Job jobToDelete = _machineController.GetPendingJobs().FirstOrDefault(j => j.JobName == jobName);
                if (jobToDelete != null)
                {
                    _machineController.RemoveJob(jobToDelete);
                    _databaseHelper.DeleteJob(jobToDelete.JobId);
                    MessageBox.Show($"Job {jobName} gelöscht.");
                    UpdateJobList();
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie einen Job aus der Liste aus.");
            }
        }

        private void ShowErrorMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                SignalLightEllipse.Fill = Brushes.Red;
                StatusLabel.Content = "Error";
            });
        }

        private void FixMachineErrorButton_Click(object sender, RoutedEventArgs e)
        {
            string errorCode = Microsoft.VisualBasic.Interaction.InputBox("Fehlercode eingeben:", "Fehlerbehebung", "");
            _machineController.FixMachineError(errorCode);
            UpdateStatus();
        }
    }
}
