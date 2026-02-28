using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogStream.Core.Abstractions;
using LogStream.Core.Models;
using LogStream.ViewModels;
using Moq;
using Xunit;
using System.Collections.ObjectModel;

namespace LogStream.Maui.Tests.ViewModels
{
    public class MainPageViewModelTests
    {
        [Fact]
        public void LoadUploads_CallsRepositoryAndPopulatesUploads()
        {
            var uploads = new List<LogUpload>
            {
                new LogUpload { Id = Guid.NewGuid(), FileName = "log1.txt", CreatedUtc = DateTimeOffset.UtcNow },
                new LogUpload { Id = Guid.NewGuid(), FileName = "log2.txt", CreatedUtc = DateTimeOffset.UtcNow }
            };
            var repoMock = new Mock<ILogRepository>();
            repoMock.Setup(r => r.GetUploadsAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(uploads);
            var vm = new MainPageViewModel(repoMock.Object);
            Assert.NotNull(vm.Uploads);
            Assert.Equal(2, vm.Uploads.Count);
        }

        [Fact]
        public async Task ProcessLogEntryAsync_CreatesUploadAndAddsEntries()
        {
            var repoMock = new Mock<ILogRepository>();
            repoMock.Setup(r => r.CreateUploadAsync(It.IsAny<LogUpload>(), default)).ReturnsAsync((LogUpload u, System.Threading.CancellationToken ct) => u);
            repoMock.Setup(r => r.AddEntriesAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<LogEntry>>(), default)).Returns(Task.CompletedTask);
            var vm = new MainPageViewModel(repoMock.Object);
            string testFilePath = "test.log";
            System.IO.File.WriteAllText(testFilePath, "entry1\nentry2");
            await vm.GetType().GetMethod("ProcessLogEntryAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(vm, new object[] { testFilePath });
            repoMock.Verify(r => r.CreateUploadAsync(It.IsAny<LogUpload>(), default), Times.Once);
            repoMock.Verify(r => r.AddEntriesAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<LogEntry>>(), default), Times.Once);
            System.IO.File.Delete(testFilePath);
        }

        [Fact]
        public void ApplyFilter_FiltersUploadsByFileName()
        {
            var uploads = new List<LogUpload>
            {
                new LogUpload { Id = Guid.NewGuid(), FileName = "log1.txt", CreatedUtc = DateTimeOffset.UtcNow },
                new LogUpload { Id = Guid.NewGuid(), FileName = "error.txt", CreatedUtc = DateTimeOffset.UtcNow }
            };
            var repoMock = new Mock<ILogRepository>();
            repoMock.Setup(r => r.GetUploadsAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(uploads);
            var vm = new MainPageViewModel(repoMock.Object);
            vm.FilterText = "error";
            vm.ApplyFilter();
            Assert.Single(vm.Uploads);
            Assert.Equal("error.txt", vm.Uploads[0].FileName);
        }
    }
}
