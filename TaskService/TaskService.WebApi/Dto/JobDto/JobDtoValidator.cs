using FluentValidation;

namespace TaskService.WebApi.Dto;

public class JobDtoValidator : AbstractValidator<JobDto>
{
    public JobDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя пользователя обязательно")
            .Length(3, 20).WithMessage("Длина имени должна быть от 3 до 20 символов");
    }
}