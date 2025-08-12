using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Interfaces;
using NotificationService.Domain;
using NotificationService.WebApi.Dtos;
using NotificationService.WebApi.Mappers;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationWithIdDto>), 200)]
    public IActionResult GetAllNotifications(Guid userId)
    {
        var notifications = _notificationService.GetNotifications(userId);
        return Ok(notifications.Select(n => n.ToNotificationWithIdDto()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    public IActionResult CreateNotification([FromBody] NotificationDto notificationDto)
    {
        var newNotification = new NewNotification(
            notificationDto.IdSender,
            notificationDto.IdRecipient,
            notificationDto.Content);

        var id = _notificationService.CreateNotification(newNotification);
        return CreatedAtAction(nameof(GetAllNotifications), new { userId = notificationDto.IdRecipient }, id);
    }

    [HttpPut("{id}/mark-as-read")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult MarkAsRead(Guid id)
    {
        _notificationService.PutNotificationAsRead(id);
        return NoContent();
    }
}