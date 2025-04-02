using Businesslogic;

public class JobManager
{
    private Queue<Job> jobQueue = new Queue<Job>();
    private Job currentJob;
    private int currentStep;
    private DbConnection _databaseHelper = new DbConnection();
    private CancellationTokenSource _cancellationTokenSource;

    public event Action<string> JobStatusChanged;
    public event Action<string> JobCompleted;
    public event Action<string> MachineFailed;

    public JobManager()
    {
        LoadJobsFromDatabase();
    }

    private void LoadJobsFromDatabase()
    {
        var jobs = _databaseHelper.GetJobs();
        foreach (var job in jobs)
        {
            jobQueue.Enqueue(job);
        }
    }

    public void AddJob(Job job)
    {
        jobQueue.Enqueue(job);
        _databaseHelper.AddJob(job);
        JobStatusChanged?.Invoke($"Job {job.JobName} hinzugefügt.");
    }

    public async Task<bool> StartJobsAsync(Machine machine)
    {
        if (jobQueue.Count == 0)
        {
            JobStatusChanged?.Invoke("Keine weiteren Jobs in der Wartenschlange.");
            return false;
        }

        currentJob = jobQueue.Dequeue();
        currentJob.Start();
        machine.Start();
        JobStatusChanged?.Invoke($"Job gestartet: Produziere {currentJob.Quantity} {currentJob.Product}.");

        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        Random random = new Random();
        bool jobCompleted = true;
        for (int i = currentStep + 1; i <= currentJob.Quantity; i++)
        {
            if (token.IsCancellationRequested)
            {
                currentJob.Stop();
                JobStatusChanged?.Invoke($"Job gestoppt: {currentJob.JobName}");
                return false;
            }

            if (random.Next(0, 10) < 1)
            {
                machine.Fail();
                jobCompleted = false;
                JobStatusChanged?.Invoke("Fehler! Die Maschine ist im Error-Zustand.");
                break;
            }
            currentStep = i;
            JobStatusChanged?.Invoke($"Produziere {currentJob.Product}... ({currentStep} von {currentJob.Quantity})");
            await Task.Delay(2000);
        }

        if (jobCompleted)
        {
            currentJob.Complete();
            machine.Stop();
            JobCompleted?.Invoke($"Job abgeschlossen: {currentJob.Quantity} {currentJob.Product} produziert.");
            return true;
        }
        return false;
    }

    public void StopCurrentJob()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void ContinueJob(Machine machine)
    {
        if (currentJob != null && currentJob.CurrentState == Job.State.Stopped)
        {
            currentJob.Start();
            StartJobsAsync(machine);
            JobStatusChanged?.Invoke($"Job fortgesetzt: {currentJob.JobName}");
        }
    }

    public void Status()
    {
        JobStatusChanged?.Invoke($"Jobs in der Wartenschlange: {jobQueue.Count}");
    }

    public List<Job> GetJobs()
    {
        return jobQueue.ToList();
    }
}