using EPMS.Application.DTOs.Comments;
using FluentValidation;

namespace EPMS.Application.Validators.Comments;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequestDto>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Komentar tidak boleh kosong.")
            .MaximumLength(2000).WithMessage("Komentar maksimal 2000 karakter.");
    }
}
