using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using TechMove.Services;
using Xunit;

namespace TechMove.Tests
{
    public class FileServiceTests
    {
        private readonly string _testRootPath;

        public FileServiceTests()
        {
            _testRootPath = Path.Combine(Path.GetTempPath(), "TechMoveTests");
            Directory.CreateDirectory(_testRootPath);
        }

        private FileService CreateFileService()
        {
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(_testRootPath);

            return new FileService(envMock.Object);
        }

        private IFormFile CreateMockFormFile(string fileName, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "file", fileName);
        }

        [Fact]
        public async Task SavePdfAsync_ShouldSucceed_ForValidPdfFile()
        {
            // Arrange
            var service = CreateFileService();
            var file = CreateMockFormFile("agreement.pdf", "dummy pdf content");

            // Act
            var result = await service.SavePdfAsync(file);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.FilePath);
            Assert.Equal("agreement.pdf", result.OriginalFileName);
            Assert.True(string.IsNullOrEmpty(result.ErrorMessage));
        }

        [Fact]
        public async Task SavePdfAsync_ShouldFail_ForInvalidFileExtension()
        {
            // Arrange
            var service = CreateFileService();
            var file = CreateMockFormFile("virus.exe", "bad file");

            // Act
            var result = await service.SavePdfAsync(file);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.FilePath);
            Assert.Equal("Only PDF files are allowed.", result.ErrorMessage);
        }

        [Fact]
        public async Task SavePdfAsync_ShouldFail_WhenFileIsNull()
        {
            // Arrange
            var service = CreateFileService();

            // Act
            var result = await service.SavePdfAsync(null!);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.FilePath);
            Assert.Equal("Please select a PDF file.", result.ErrorMessage);
        }
    }
}