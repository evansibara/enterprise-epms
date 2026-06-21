using EPMS.Application.Interfaces.Services;
using EPMS.Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace EPMS.Infrastructure.Files;

/// <summary>
/// Implementasi awal IFileStorageService dengan local disk (section 4.7).
/// Interface IFileStorageService didesain generik (Stream in/out) sehingga
/// nanti tinggal buat S3FileStorageService tanpa mengubah Application/Domain
/// layer maupun caller (AttachmentService).
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _settings;

    public LocalFileStorageService(IOptions<FileStorageSettings> options)
    {
        _settings = options.Value;
        Directory.CreateDirectory(_settings.RootPath);
    }

    public async Task<StoredFileResult> SaveAsync(
        Stream fileStream, string originalFileName, string mimeType, long sizeBytes,
        CancellationToken cancellationToken = default)
    {
        if (sizeBytes > _settings.MaxFileSizeBytes)
        {
            throw new DomainValidationException(
                $"Ukuran file melebihi batas maksimum {_settings.MaxFileSizeBytes / 1024 / 1024} MB.");
        }

        if (!_settings.AllowedMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase))
        {
            throw new DomainValidationException($"Tipe file '{mimeType}' tidak diizinkan.");
        }

        var safeExtension = Path.GetExtension(originalFileName);
        var generatedFileName = $"{Guid.NewGuid()}{safeExtension}";
        var fullPath = Path.Combine(_settings.RootPath, generatedFileName);

        await using var destination = File.Create(fullPath);
        await fileStream.CopyToAsync(destination, cancellationToken);

        return new StoredFileResult(
            FilePath: fullPath,
            FileName: originalFileName,
            MimeType: mimeType,
            SizeBytes: sizeBytes);
    }

    public Task<Stream> GetAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            throw new NotFoundException("File tidak ditemukan di storage.");
        }

        Stream stream = File.OpenRead(filePath);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
