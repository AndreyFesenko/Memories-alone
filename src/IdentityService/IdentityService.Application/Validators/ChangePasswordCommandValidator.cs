using FluentValidation;
using IdentityService.Application.Commands;

namespace IdentityService.Application.Validators;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OldPassword).NotEmpty().MinimumLength(6);
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Пароль должен быть не менее 8 символов")
            .Matches("[A-Z]").WithMessage("Пароль должен содержать заглавную букву")
            .Matches("[a-z]").WithMessage("Пароль должен содержать строчную букву")
            .Matches("[0-9]").WithMessage("Пароль должен содержать цифру");
    }
}
