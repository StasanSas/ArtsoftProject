namespace TaskService.Domain;

public class Job
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public Job(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public Job(Guid id, string name, string description)
        : this(name, description)
    {
        Id = id;
    }

}