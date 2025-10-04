using FluentValidation;
using ProfileService.Application.Commands;

namespace ProfileService.Application.Validators;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.DisplayName).MaximumLength(100);
        RuleFor(x => x.Bio).MaximumLength(1000);
        RuleFor(x => x.AccessMode).NotEmpty().Must(m => m is "Anytime" or "AfterDeath");
    }
}
