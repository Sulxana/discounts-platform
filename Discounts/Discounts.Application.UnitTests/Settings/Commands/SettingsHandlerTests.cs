using Discounts.Application.Settings.Commands.UpdateSetting;
using Discounts.Application.Settings.Interfaces;
using Discounts.Domain.Settings;
using Moq;
using Xunit;

namespace Discounts.Application.UnitTests.Settings.Commands
{
    public class SettingsHandlerTests
    {
        private readonly Mock<IGlobalSettingRepository> _repositoryMock;
        private readonly Mock<IGlobalSettingsService> _settingsServiceMock;

        public SettingsHandlerTests()
        {
            _repositoryMock = new Mock<IGlobalSettingRepository>();
            _settingsServiceMock = new Mock<IGlobalSettingsService>();
        }

        [Fact]
        public async Task UpdateSetting_ShouldUpdateValueAndClearCache_WhenSettingExists()
        {
            // Arrange
            var existingSetting = new GlobalSetting("merchant_edit_hours", "24", "Time window", SettingType.String);
            var command = new UpdateSettingCommand("Merchant_Edit_Hours", "48");
            
            var handler = new UpdateSettingHandler(_repositoryMock.Object, _settingsServiceMock.Object);

            _repositoryMock.Setup(r => r.GetByKeyAsync(It.IsAny<CancellationToken>(), "merchant_edit_hours"))
                .ReturnsAsync(existingSetting);
            _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("48", existingSetting.Value);
            _repositoryMock.Verify(r => r.GetByKeyAsync(It.IsAny<CancellationToken>(), "merchant_edit_hours"), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _settingsServiceMock.Verify(s => s.RemoveFromCache("merchant_edit_hours"), Times.Once);
        }

        [Fact]
        public async Task UpdateSetting_ShouldThrowInvalidOperationException_WhenSettingDoesNotExist()
        {
            // Arrange
            var command = new UpdateSettingCommand("non_existent_key", "48");
            var handler = new UpdateSettingHandler(_repositoryMock.Object, _settingsServiceMock.Object);

            _repositoryMock.Setup(r => r.GetByKeyAsync(It.IsAny<CancellationToken>(), "non_existent_key"))
                .ReturnsAsync((GlobalSetting?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Contains("not found", ex.Message);
            
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _settingsServiceMock.Verify(s => s.RemoveFromCache(It.IsAny<string>()), Times.Never);
        }
    }
}
