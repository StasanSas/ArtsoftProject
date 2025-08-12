using System.ComponentModel.DataAnnotations;

namespace NotificationService.WebApi.Dtos;

public class NotificationDto
{
    [Required(ErrorMessage = "Sender Id is required")]
    public Guid IdSender { get; set; }
    
    [Required(ErrorMessage = "Recipient Id is required")]
    public Guid IdRecipient { get; set; }
    
    [Required(ErrorMessage = "Content is required")]
    [MaxLength(1000, ErrorMessage = "Content must be less than 1000 characters")]
    public string Content { get; set; }
}