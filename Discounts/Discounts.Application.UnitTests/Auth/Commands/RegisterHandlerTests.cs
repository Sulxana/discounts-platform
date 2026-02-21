using Discounts.Application.Auth.Commands.Register;
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
    public class RegisterHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IValidator<RegisterCommand>> _validatorMock;
        private readonly Mock<IOptions<JwtSettings>> _settingsMock;
        private readonly RegisterHandler _handler;

        public RegisterHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _validatorMock = new Mock<IValidator<RegisterCommand>>();
            
            var settings = new JwtSettings { RefreshTokenDays = 7 };
            _settingsMock = new Mock<IOptions<JwtSettings>>();
            _settingsMock.Setup(s => s.Value).Returns(settings);

            _handler = new RegisterHandler(
                _identityServiceMock.Object,
                _jwtGeneratorMock.Object,
                _authRepositoryMock.Object,
                _validatorMock.Object,
                _settingsMock.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnAuthResponse_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var command = new RegisterCommand
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };
            
            var userId = Guid.NewGuid();
            var roles = new List<string> { Roles.Customer };
            var accessToken = "access.token.here";
            var jwtId = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(15);
            var refreshRaw = "refresh_token_string";

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RegisterCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            
            _identityServiceMock.Setup(i => i.CreateUserAsync(command.Email, command.Password, command.FirstName, command.LastName, Roles.Customer))
                .ReturnsAsync((true, null, userId));
            
            _identityServiceMock.Setup(i => i.GetUserRolesAsync(userId))
                .ReturnsAsync(roles);
            
            _jwtGeneratorMock.Setup(j => j.GenerateAccessToken(userId, command.Email, roles))
                .Returns((accessToken, jwtId, expiresAt));
            
            _jwtGeneratorMock.Setup(j => j.GenerateRefreshToken())
                .Returns(refreshRaw);

            // Act
            var result = await _handler.Register(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be(accessToken);
            result.RefreshToken.Should().Be(refreshRaw);
            result.ExpiresAt.Should().Be(expiresAt);

            _authRepositoryMock.Verify(a => a.AddRefreshTokenAsync(It.IsAny<CancellationToken>(), It.Is<RefreshToken>(rt => rt.UserId == userId && rt.JwtId == jwtId)), Times.Once);
            _authRepositoryMock.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Register_ShouldThrowInvalidOperationException_WhenUserCreationFails()
        {
            // Arrange
            var command = new RegisterCommand
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RegisterCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            
            _identityServiceMock.Setup(i => i.CreateUserAsync(command.Email, command.Password, command.FirstName, command.LastName, Roles.Customer))
                .ReturnsAsync((false, "Email already in use", Guid.Empty));

            // Act
            var act = async () => await _handler.Register(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Email already in use");
            
            _jwtGeneratorMock.Verify(j => j.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IList<string>>()), Times.Never);
            _authRepositoryMock.Verify(a => a.AddRefreshTokenAsync(It.IsAny<CancellationToken>(), It.IsAny<RefreshToken>()), Times.Never);
            _authRepositoryMock.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Register_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new RegisterCommand { Email = "invalid" };
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Email", "Invalid format") };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RegisterCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.Register(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            
            _identityServiceMock.Verify(i => i.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
