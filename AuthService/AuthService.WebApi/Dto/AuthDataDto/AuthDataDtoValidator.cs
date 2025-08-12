using FluentValidation;

namespace AuthService.WebApi.Dto;

public class AuthDataDtoValidator : AbstractValidator<AuthDataDto>
{
    public AuthDataDtoValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин не может быть пустым")
            .Length(3, 30).WithMessage("Длина логина должна быть от 3 до 20 символов")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Логин может содержать только буквы, цифры и подчеркивание");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не может быть пустым")
            .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
            .MaximumLength(30).WithMessage("Пароль не должен превышать 30 символов")
            .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
            .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
            .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру")
            .Matches("[^a-zA-Z0-9]").WithMessage("Пароль должен содержать хотя бы один спецсимвол");
    }
}