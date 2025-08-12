namespace TaskService.WebApi.Dto;

public class WorkflowDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public IEnumerable<Guid> Executors { get; set; }
}