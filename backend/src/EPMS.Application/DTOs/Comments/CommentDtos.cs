namespace EPMS.Application.DTOs.Comments;

public class CommentResponseDto
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }

    public string Content { get; set; } = string.Empty;

    public Guid CreatedByUserId { get; set; }

    public string? CreatedByUserName { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CreateCommentRequestDto
{
    public string Content { get; set; } = string.Empty;
}
