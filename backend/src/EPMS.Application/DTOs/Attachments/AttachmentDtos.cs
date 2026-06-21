namespace EPMS.Application.DTOs.Attachments;

public class AttachmentResponseDto
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public DateTime CreatedAt { get; set; }
}
