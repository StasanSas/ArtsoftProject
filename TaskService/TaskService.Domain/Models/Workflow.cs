namespace TaskService.Domain;

public class Workflow
{
    public Job job { get; private set; }
    private Dictionary<Guid,Executor> _executors { get; set; }
    
    public Executor this[Guid index] => _executors[index]; 
    
    public IEnumerable<Executor> Executors => _executors.Values; 
    
    public bool Contains(Guid executorId) => _executors.ContainsKey(executorId); 
    
    public Workflow(Job job, IEnumerable<Executor> executors)
    {
        this.job = job;
        foreach (var executor in executors)
        {
            _executors.Add(executor.Id, executor);
        }
    }

    public void AddExecutor(Executor executor)
    {
        _executors.Add(executor.Id, executor);
    }
    
    public void RemoveExecutor(Executor executor)
    {
        _executors.Remove(executor.Id);
    }
}