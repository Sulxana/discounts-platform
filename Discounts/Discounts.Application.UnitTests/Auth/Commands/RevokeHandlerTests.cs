using Discounts.Application.Auth.Commands.Revoke;
using Discounts.Application.Auth.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Domain.Auth;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Discounts.Application.UnitTests.Auth.Commands
{
    public class RevokeHandlerTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IValidator<RevokeCommand>> _validatorMock;
        private readonly RevokeHandler _handler;

        public RevokeHandlerTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _validatorMock = new Mock<IValidator<RevokeCommand>>();

            _handler = new RevokeHandler(_authRepositoryMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task RevokeToken_ShouldRevokeToken_WhenTokenIsValidAndActive()
        {
            // Arrange
            var rawToken = "valid_refresh_token";
            var command = new RevokeCommand { RefreshToken = rawToken };
            var hash = TokenHasher.Sha256Base64(rawToken);
            var userId = Guid.NewGuid();
            var jwtId = Guid.NewGuid().ToString();
            var storedToken = new RefreshToken(userId, hash, jwtId, DateTime.UtcNow.AddDays(7));

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RevokeCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _authRepositoryMock.Setup(a => a.GetRefreshTokenByHashAsync(It.IsAny<CancellationToken>(), hash))
                .ReturnsAsync(storedToken);

            // Act
            await _handler.RevokeToken(command, CancellationToken.None);

            // Assert
            storedToken.RevokedAt.Should().NotBeNull();
            _authRepositoryMock.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RevokeToken_ShouldDoNothing_WhenTokenIsNotFound()
        {
            // Arrange
            var rawToken = "missing_refresh_token";
            var command = new RevokeCommand { RefreshToken = rawToken };
            var hash = TokenHasher.Sha256Base64(rawToken);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RevokeCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _authRepositoryMock.Setup(a => a.GetRefreshTokenByHashAsync(It.IsAny<CancellationToken>(), hash))
                .ReturnsAsync((RefreshToken?)null);

            // Act
            await _handler.RevokeToken(command, CancellationToken.None);

            // Assert
            _authRepositoryMock.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RevokeToken_ShouldDoNothing_WhenTokenIsAlreadyInactive()
        {
            // Arrange
            var rawToken = "inactive_refresh_token";
            var command = new RevokeCommand { RefreshToken = rawToken };
            var hash = TokenHasher.Sha256Base64(rawToken);
            var userId = Guid.NewGuid();
            var jwtId = Guid.NewGuid().ToString();
            var storedToken = new RefreshToken(userId, hash, jwtId, DateTime.UtcNow.AddDays(7));
            storedToken.Revoke(); // Make it inactive

            // Capture the time it was revoked to ensure it wasn't revoked again (which would update the time)
            var originalRevokedAt = storedToken.RevokedAt;

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RevokeCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _authRepositoryMock.Setup(a => a.GetRefreshTokenByHashAsync(It.IsAny<CancellationToken>(), hash))
                .ReturnsAsync(storedToken);

            // Act
            await _handler.RevokeToken(command, CancellationToken.None);

            // Assert
            storedToken.RevokedAt.Should().Be(originalRevokedAt);
            _authRepositoryMock.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RevokeToken_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new RevokeCommand { RefreshToken = string.Empty };
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("RefreshToken", "Required") };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RevokeCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.RevokeToken(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();

            _authRepositoryMock.Verify(a => a.GetRefreshTokenByHashAsync(It.IsAny<CancellationToken>(), It.IsAny<string>()), Times.Never);
            _authRepositoryMock.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
