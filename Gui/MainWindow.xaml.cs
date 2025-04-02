using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Businesslogic
{
    public partial class MainWindow : Window
    {
        private MachineController _machineController;
        private DbConnection _databaseHelper;

        public MainWindow()
        {
            InitializeComponent();
            Businesslogic.Machine machine = new Businesslogic.Machine();
            JobManager jobManager = new JobManager();
            _machineController = new MachineController(machine, jobManager, this);
            _databaseHelper = new DbConnection();

            _machineController.JobStatusChanged += UpdateJobStatus;
            _machineController.JobCompleted += UpdateJobStatus;
            _machineController.MachineFailed += ShowErrorMessage;

            SignalLightEllipse.Fill = Brushes.Yellow;
            StatusLabel.Content = "Ready";

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

            Job job = new Job(jobName, product, quantity);
            _machineController.AddJob(job);
            _databaseHelper.AddJob(job); // Job zur Datenbank hinzufügen
            MessageBox.Show($"Job {jobName} hinzugefügt: {quantity} {product}");
            UpdateJobList();
        }

        private void StartJobsButton_Click(object sender, RoutedEventArgs e)
        {
            _machineController.StartJobs();
            UpdateStatus();
        }

        private void StopCurrentJobButton_Click(object sender, RoutedEventArgs e)
        {
            _machineController.StopCurrentJob();
        }

        private void ContinueJobButton_Click(object sender, RoutedEventArgs e)
        {
            _machineController.ContinueJob();
        }

        private void GetJobStatusButton_Click(object sender, RoutedEventArgs e)
        {
            _machineController.GetJobStatus();
        }

        private void UpdateStatus()
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
            var jobs = _machineController.GetJobs();
            foreach (var job in jobs)
            {
                JobsListBox.Items.Add($"{job.JobName}: {job.Quantity} {job.Product}");
            }
        }

        private void UpdateJobStatus(string status)
        {
            Dispatcher.Invoke(() => JobStatusLabel.Content = status);
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
    }
}