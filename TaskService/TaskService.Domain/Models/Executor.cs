namespace TaskService.Domain;

public class Executor
{
    public Guid Id { get; private set; }

    public Executor(Guid id)
    {
        Id = id;
    }
}