using Discounts.Application.Auth.DTOs;
using Discounts.Application.Auth.Queries.WhoAmI;
using Discounts.Application.Common.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Discounts.Application.UnitTests.Auth.Queries
{
    public class WhoAmIHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly WhoAmIHandler _handler;

        public WhoAmIHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _identityServiceMock = new Mock<IIdentityService>();

            _handler = new WhoAmIHandler(_currentUserServiceMock.Object, _identityServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnWhoAmIResponse_WhenUserIsAuthenticated()
        {
            // Arrange
            var query = new WhoAmIQuery();
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var roles = new List<string> { "Customer", "Admin" };

            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);
            _identityServiceMock.Setup(i => i.GetUserByIdAsync(userId))
                .ReturnsAsync((true, userId, email, roles));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.Email.Should().Be(email);
            result.Roles.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var query = new WhoAmIQuery();
            _currentUserServiceMock.Setup(c => c.UserId).Returns((Guid?)null);

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User is not authenticated.");
            
            _identityServiceMock.Verify(i => i.GetUserByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenUserRetrievalFails()
        {
            // Arrange
            var query = new WhoAmIQuery();
            var userId = Guid.NewGuid();

            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);
            _identityServiceMock.Setup(i => i.GetUserByIdAsync(userId))
                .ReturnsAsync((false, Guid.Empty, string.Empty, new List<string>()));

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Failed to retrieve user information.");
        }
    }
}
