using FluentValidation;
using NotificationService.Application.Commands;

public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.Recipient).NotEmpty().WithMessage("Recipient is required");
        RuleFor(x => x.Subject).NotEmpty().WithMessage("Subject is required");
        RuleFor(x => x.Body).NotEmpty().WithMessage("Body is required");
    }
}
