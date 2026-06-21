namespace EPMS.Infrastructure.Files;

public class FileStorageSettings
{
    public const string SectionName = "FileStorage";

    /// <summary>Folder root tempat file disimpan, relatif ke working directory WebApi.</summary>
    public string RootPath { get; set; } = "uploads";

    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10 MB

    public string[] AllowedMimeTypes { get; set; } =
    {
        "image/png", "image/jpeg", "image/gif", "image/webp",
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain", "text/csv",
        "application/zip"
    };
}
