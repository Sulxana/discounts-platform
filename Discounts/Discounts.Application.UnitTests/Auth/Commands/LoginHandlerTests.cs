using Discounts.Application.Auth.Commands.Login;
using Discounts.Application.Auth.Interfaces;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Domain.Auth;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Moq;

namespace Discounts.Application.UnitTests.Auth.Commands
{
    public class LoginHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IValidator<LoginCommand>> _validatorMock;
        private readonly Mock<IOptions<JwtSettings>> _settingsMock;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _validatorMock = new Mock<IValidator<LoginCommand>>();

            var settings = new JwtSettings { RefreshTokenDays = 7 };
            _settingsMock = new Mock<IOptions<JwtSettings>>();
            _settingsMock.Setup(s => s.Value).Returns(settings);

            _handler = new LoginHandler(
                _identityServiceMock.Object,
                _jwtGeneratorMock.Object,
                _authRepositoryMock.Object,
                _validatorMock.Object,
                _settingsMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnAuthResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var userId = Guid.NewGuid();
            var roles = new List<string> { Roles.Customer };
            var accessToken = "access.token.here";
            var jwtId = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(15);
            var refreshRaw = "refresh_token_string";

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<LoginCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _identityServiceMock.Setup(i => i.LoginUserAsync(command.Email, command.Password))
                .ReturnsAsync((true, userId, command.Email, roles));

            _jwtGeneratorMock.Setup(j => j.GenerateAccessToken(userId, command.Email, roles))
                .Returns((accessToken, jwtId, expiresAt));

            _jwtGeneratorMock.Setup(j => j.GenerateRefreshToken())
                .Returns(refreshRaw);

            // Act
            var result = await _handler.Login(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be(accessToken);
            result.RefreshToken.Should().Be(refreshRaw);
            result.ExpiresAt.Should().Be(expiresAt);

            _authRepositoryMock.Verify(a => a.AddRefreshTokenAsync(It.IsAny<CancellationToken>(), It.Is<RefreshToken>(rt => rt.UserId == userId && rt.JwtId == jwtId)), Times.Once);
            _authRepositoryMock.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldThrowInvalidOperationException_WhenCredentialsAreInvalid()
        {
            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "WrongPassword!"
            };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<LoginCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _identityServiceMock.Setup(i => i.LoginUserAsync(command.Email, command.Password))
                .ReturnsAsync((false, Guid.Empty, string.Empty, new List<string>()));

            // Act
            var act = async () => await _handler.Login(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Invalid Credentials");

            _jwtGeneratorMock.Verify(j => j.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IList<string>>()), Times.Never);
            _authRepositoryMock.Verify(a => a.AddRefreshTokenAsync(It.IsAny<CancellationToken>(), It.IsAny<RefreshToken>()), Times.Never);
            _authRepositoryMock.Verify(a => a.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Login_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new LoginCommand { Email = "invalid_email.com" };
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Email", "Invalid format") };

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<LoginCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.Login(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();

            _identityServiceMock.Verify(i => i.LoginUserAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
