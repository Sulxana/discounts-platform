using Discounts.Application.Common.Exceptions;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Application.Coupons.Interfaces;
using Discounts.Application.Offers.Commands.DeleteOffer;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using Discounts.Domain.Reservations;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Discounts.Application.UnitTests.Offers.Commands
{
    public class DeleteOfferHandlerTests
    {
        private readonly Mock<IOfferRepository> _repositoryMock;
        private readonly Mock<IValidator<DeleteOfferCommand>> _validatorMock;
        private readonly Mock<ICouponRepository> _couponRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly DeleteOfferHandler _handler;

        public DeleteOfferHandlerTests()
        {
            _repositoryMock = new Mock<IOfferRepository>();
            _validatorMock = new Mock<IValidator<DeleteOfferCommand>>();
            _couponRepositoryMock = new Mock<ICouponRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new DeleteOfferHandler(
                _repositoryMock.Object,
                _validatorMock.Object,
                _couponRepositoryMock.Object,
                _currentUserServiceMock.Object);
        }

        private Offer CreateValidOffer(Guid merchantId)
        {
            return new Offer(
                "Test Offer",
                "Description",
                Guid.NewGuid(),
                null,
                100m,
                50m,
                10,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(7),
                merchantId
            );
        }

        [Fact]
        public async Task DeleteOfferAsync_ShouldMarkAsDeleted_WhenCommandIsValidAndUserIsOwner()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var deleteReason = "Out of stock";
            var command = new DeleteOfferCommand(offerId, deleteReason);
            var offer = CreateValidOffer(merchantId);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DeleteOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);
            
            _currentUserServiceMock.Setup(c => c.UserId).Returns(merchantId);

            _couponRepositoryMock.Setup(c => c.HasCouponsForOfferAsync(offerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            await _handler.DeleteOfferAsync(CancellationToken.None, command);

            // Assert
            offer.IsDeleted.Should().BeTrue();
            offer.Status.Should().Be(OfferStatus.Deleted);
            offer.RejectionMessage.Should().Be(deleteReason);
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteOfferAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotOwnerOrAdmin()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var command = new DeleteOfferCommand(offerId);
            var offer = CreateValidOffer(merchantId);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DeleteOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);
            
            _currentUserServiceMock.Setup(c => c.UserId).Returns(otherUserId);
            _currentUserServiceMock.Setup(c => c.IsInRole(Roles.Administrator)).Returns(false);

            // Act
            var act = async () => await _handler.DeleteOfferAsync(CancellationToken.None, command);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOfferAsync_ShouldThrowInvalidOperationException_WhenOfferHasActiveReservations()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var command = new DeleteOfferCommand(offerId);
            var offer = CreateValidOffer(merchantId);

            // Use reflection to add an active reservation since Reservations collection is ICollection protected
            var reservation = new Reservation(offerId, Guid.NewGuid(), 1, DateTime.UtcNow.AddMinutes(15));
            var reservationsProp = typeof(Offer).GetProperty("Reservations");
            var reservations = (ICollection<Reservation>)reservationsProp!.GetValue(offer)!;
            reservations.Add(reservation);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DeleteOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);
            
            _currentUserServiceMock.Setup(c => c.UserId).Returns(merchantId);

            // Act
            var act = async () => await _handler.DeleteOfferAsync(CancellationToken.None, command);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot delete offer with active reservations. Please wait for them to expire or cancel them.");
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOfferAsync_ShouldThrowInvalidOperationException_WhenOfferHasSoldCoupons()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var command = new DeleteOfferCommand(offerId);
            var offer = CreateValidOffer(merchantId);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DeleteOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);
            
            _currentUserServiceMock.Setup(c => c.UserId).Returns(merchantId);

            // Simulate sold coupons
            _couponRepositoryMock.Setup(c => c.HasCouponsForOfferAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _handler.DeleteOfferAsync(CancellationToken.None, command);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot delete offer that has sold coupons.");
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOfferAsync_ShouldThrowNotFoundException_WhenOfferDoesNotExist()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new DeleteOfferCommand(offerId);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DeleteOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync((Offer?)null);

            // Act
            var act = async () => await _handler.DeleteOfferAsync(CancellationToken.None, command);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOfferAsync_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new DeleteOfferCommand(Guid.Empty);
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Id", "Id is required") };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DeleteOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.DeleteOfferAsync(CancellationToken.None, command);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            _repositoryMock.Verify(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
