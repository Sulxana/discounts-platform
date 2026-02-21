using Discounts.Application.Auth.Commands.RefreshTokens;
using Discounts.Application.Auth.DTOs;
using Discounts.Application.Auth.Interfaces;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Domain.Auth;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Discounts.Application.UnitTests.Auth.Commands
{
    public class RefreshTokenHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IValidator<RefreshTokenCommand>> _validatorMock;
        private readonly Mock<IOptions<JwtSettings>> _settingsMock;
        private readonly RefreshTokenHandler _handler;

        public RefreshTokenHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _validatorMock = new Mock<IValidator<RefreshTokenCommand>>();
            
            var settings = new JwtSettings { RefreshTokenDays = 7 };
            _settingsMock = new Mock<IOptions<JwtSettings>>();
            _settingsMock.Setup(s => s.Value).Returns(settings);

            _handler = new RefreshTokenHandler(
                _identityServiceMock.Object,
                _jwtGeneratorMock.Object,
                _authRepositoryMock.Object,
                _validatorMock.Object,
                _settingsMock.Object);
        }

        [Fact]
        public async Task CreateRefreshToken_ShouldReturnNewAuthResponse_WhenTokenIsValid()
        {
            // Arrange
            var rawToken = "valid_refresh_token";
            var command = new RefreshTokenCommand { RefreshToken = rawToken };
            var hash = TokenHasher.Sha256Base64(rawToken);
            
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var roles = new List<string> { Roles.Customer };
            
            var oldJwtId = Guid.NewGuid().ToString();
            var storedToken = new RefreshToken(userId, hash, oldJwtId, DateTime.UtcNow.AddDays(7));
            
            var newAccessToken = "new.access.token";
            var newJwtId = Guid.NewGuid().ToString();
            var newExpiresAt = DateTime.UtcNow.AddMinutes(15);
            var newRefreshRaw = "new_refresh_token";

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RefreshTokenCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            
            _authRepositoryMock.Setup(a => a.GetRefreshTokenByHashAsync(It.IsAny<CancellationToken>(), hash))
                .ReturnsAsync(storedToken);
            
            _identityServiceMock.Setup(i => i.GetUserByIdAsync(userId))
                .ReturnsAsync((true, userId, email, roles));
            
            _jwtGeneratorMock.Setup(j => j.GenerateAccessToken(userId, email, roles))
                .Returns((newAccessToken, newJwtId, newExpiresAt));
            
            _jwtGeneratorMock.Setup(j => j.GenerateRefreshToken())
                .Returns(newRefreshRaw);

            // Act
            var result = await _handler.CreateRefreshToken(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be(newAccessToken);
            result.RefreshToken.Should().Be(newRefreshRaw);
            result.ExpiresAt.Should().Be(newExpiresAt);

            storedToken.UsedAt.Should().NotBeNull();

            _authRepositoryMock.Verify(a => a.AddRefreshTokenAsync(It.IsAny<CancellationToken>(), It.Is<RefreshToken>(rt => rt.UserId == userId && rt.JwtId == newJwtId)), Times.Once);
            _authRepositoryMock.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRefreshToken_ShouldThrowInvalidOperationException_WhenTokenIsNotFound()
        {
            // Arrange
            var rawToken = "invalid_refresh_token";
            var command = new RefreshTokenCommand { RefreshToken = rawToken };
            var hash = TokenHasher.Sha256Base64(rawToken);

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RefreshTokenCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            
            _authRepositoryMock.Setup(a => a.GetRefreshTokenByHashAsync(It.IsAny<CancellationToken>(), hash))
                .ReturnsAsync((RefreshToken?)null);

            // Act
            var act = async () => await _handler.CreateRefreshToken(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Invalid refresh token");
        }

        [Fact]
        public async Task CreateRefreshToken_ShouldThrowInvalidOperationException_WhenTokenIsNotActive()
        {
            // Arrange
            var rawToken = "inactive_refresh_token";
            var command = new RefreshTokenCommand { RefreshToken = rawToken };
            var hash = TokenHasher.Sha256Base64(rawToken);
            var storedToken = new RefreshToken(Guid.NewGuid(), hash, Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(7));
            storedToken.MarkAsUsed(); // Make it inactive

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RefreshTokenCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            
            _authRepositoryMock.Setup(a => a.GetRefreshTokenByHashAsync(It.IsAny<CancellationToken>(), hash))
                .ReturnsAsync(storedToken);

            // Act
            var act = async () => await _handler.CreateRefreshToken(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Refresh token is not active");
        }

        [Fact]
        public async Task CreateRefreshToken_ShouldThrowInvalidOperationException_WhenUserIsNotFound()
        {
            // Arrange
            var rawToken = "valid_refresh_token";
            var command = new RefreshTokenCommand { RefreshToken = rawToken };
            var hash = TokenHasher.Sha256Base64(rawToken);
            var userId = Guid.NewGuid();
            var storedToken = new RefreshToken(userId, hash, Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(7));

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RefreshTokenCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            
            _authRepositoryMock.Setup(a => a.GetRefreshTokenByHashAsync(It.IsAny<CancellationToken>(), hash))
                .ReturnsAsync(storedToken);
            
            _identityServiceMock.Setup(i => i.GetUserByIdAsync(userId))
                .ReturnsAsync((false, Guid.Empty, string.Empty, new List<string>())); // User not found

            // Act
            var act = async () => await _handler.CreateRefreshToken(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("User Not Found");
        }

        [Fact]
        public async Task CreateRefreshToken_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = string.Empty };
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("RefreshToken", "Required") };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RefreshTokenCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.CreateRefreshToken(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            
            _authRepositoryMock.Verify(a => a.GetRefreshTokenByHashAsync(It.IsAny<CancellationToken>(), It.IsAny<string>()), Times.Never);
        }
    }
}
