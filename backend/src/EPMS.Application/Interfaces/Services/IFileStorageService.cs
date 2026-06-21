namespace EPMS.Application.Interfaces.Services;

public record StoredFileResult(string FilePath, string FileName, string MimeType, long SizeBytes);

/// <summary>
/// Abstraksi penyimpanan file. Implementasi awal pakai local disk (lihat
/// EPMS.Infrastructure.Files.LocalFileStorageService); didesain agar mudah
/// diganti ke AWS S3 nantinya tanpa mengubah Application/Domain layer.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Simpan file, validasi MIME type & ukuran dilakukan di implementasi.
    /// </summary>
    Task<StoredFileResult> SaveAsync(
        Stream fileStream, string originalFileName, string mimeType, long sizeBytes,
        CancellationToken cancellationToken = default);

    Task<Stream> GetAsync(string filePath, CancellationToken cancellationToken = default);

    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);
}
