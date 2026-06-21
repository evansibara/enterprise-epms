using EPMS.Application.DTOs.Tasks;
using FluentValidation;

namespace EPMS.Application.Validators.Tasks;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequestDto>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Judul task wajib diisi.")
            .MaximumLength(250);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority wajib diisi.");
    }
}

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequestDto>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Judul task wajib diisi.")
            .MaximumLength(250);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority wajib diisi.");
    }
}

public class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequestDto>
{
    public UpdateTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status wajib diisi.");
    }
}

public class AssignTaskRequestValidator : AbstractValidator<AssignTaskRequestDto>
{
    public AssignTaskRequestValidator()
    {
        RuleFor(x => x.AssigneeId)
            .NotEmpty().WithMessage("AssigneeId wajib diisi.");
    }
}
