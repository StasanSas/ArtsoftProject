namespace TaskService.Domain;

public class NewJob
{
    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public NewJob(string name, string description)
    {
        Name = name;
        Description = description;
    }
    

}