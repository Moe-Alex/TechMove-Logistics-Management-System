using Microsoft.AspNetCore.Http;

namespace TechMove.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<(bool Success, string? FilePath, string? OriginalFileName, string ErrorMessage)> SavePdfAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, null, null, "Please select a PDF file.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (extension != ".pdf")
            {
                return (false, null, null, "Only PDF files are allowed.");
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "agreements");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/agreements/{uniqueFileName}";

            return (true, relativePath, file.FileName, string.Empty);
        }
    }
}