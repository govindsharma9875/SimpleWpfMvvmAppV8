namespace SimpleWpfToMvcApp.Web.Services;

public interface IFileUploadService
{
    Task<string?> UploadFileAsync(IFormFile file, string uploadPath = "uploads");
    Task<bool> DeleteFileAsync(string filePath);
    bool IsValidImageFile(IFormFile file);
    string GetUniqueFileName(string originalFileName);
}

public class FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger) : IFileUploadService
{
    private readonly IWebHostEnvironment _environment = environment;
    private readonly ILogger<FileUploadService> _logger = logger;
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp"];
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public async Task<string?> UploadFileAsync(IFormFile file, string uploadPath = "uploads")
    {
        try
        {
            if (!IsValidImageFile(file))
            {
                _logger.LogWarning("Invalid file type or size for file: {FileName}", file.FileName);
                return null;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, uploadPath);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = GetUniqueFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            _logger.LogInformation("File uploaded successfully: {FileName}", uniqueFileName);
            return Path.Combine(uploadPath, uniqueFileName).Replace('\\', '/');
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
            return null;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath)) return true;

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                return true;
            }
            return true; // File doesn't exist, consider it deleted
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return false;
        }
    }

    public bool IsValidImageFile(IFormFile file)
    {
        if (file == null || file.Length == 0) return false;
        if (file.Length > MaxFileSize) return false;

        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        return !string.IsNullOrEmpty(extension) && _allowedExtensions.Contains(extension);
    }

    public string GetUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var fileName = Path.GetFileNameWithoutExtension(originalFileName);
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        return $"{fileName}_{uniqueId}{extension}";
    }
}
