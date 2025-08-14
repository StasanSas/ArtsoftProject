namespace TaskService.Domain;

public class Job
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public bool IsDeleted { get; private set; }
    
    
    public Job(Guid id, string name, string description, bool isDeleted)
    {
        Id = id;
        Name = name;
        Description = description;
        IsDeleted = isDeleted;
    }

}