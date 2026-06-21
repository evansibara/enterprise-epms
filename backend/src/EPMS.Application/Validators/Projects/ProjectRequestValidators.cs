using EPMS.Application.DTOs.Projects;
using FluentValidation;

namespace EPMS.Application.Validators.Projects;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequestDto>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama project wajib diisi.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Deadline)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.Deadline.HasValue)
            .WithMessage("Deadline harus di masa depan.");
    }
}

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequestDto>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama project wajib diisi.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status wajib diisi.");
    }
}
