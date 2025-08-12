using TaskService.Domain;

namespace TaskService.Application.Interfaces;

public interface IJobEventPublisher
{
    public void Subscribe(Action<Guid,EventType> metod);
}