using Discounts.Application.Common.Exceptions;
using Discounts.Application.Offers.Commands.RejectOffer;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Discounts.Application.UnitTests.Offers.Commands
{
    public class RejectOfferHandlerTests
    {
        private readonly Mock<IOfferRepository> _repositoryMock;
        private readonly Mock<IValidator<RejectOfferCommand>> _validatorMock;
        private readonly RejectOfferHandler _handler;

        public RejectOfferHandlerTests()
        {
            _repositoryMock = new Mock<IOfferRepository>();
            _validatorMock = new Mock<IValidator<RejectOfferCommand>>();

            _handler = new RejectOfferHandler(_repositoryMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task RejectOfferAsync_ShouldChangeStatusToRejectedAndSetMessage_WhenOfferExistsAndIsPending()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var rejectionReason = "Inappropriate content";
            var command = new RejectOfferCommand(offerId, rejectionReason);

            var offer = new Offer(
                "Test Offer",
                "Description",
                Guid.NewGuid(),
                null,
                100m,
                50m,
                10,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(7),
                Guid.NewGuid()
            );

            var idProperty = typeof(Offer).GetProperty("Id");
            idProperty?.SetValue(offer, offerId);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RejectOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            await _handler.RejectOfferAsync(CancellationToken.None, command);

            // Assert
            offer.Status.Should().Be(OfferStatus.Rejected);
            offer.RejectionMessage.Should().Be(rejectionReason);
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RejectOfferAsync_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new RejectOfferCommand(Guid.Empty, "");
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Id", "Id is required") };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RejectOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.RejectOfferAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            _repositoryMock.Verify(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RejectOfferAsync_ShouldThrowNotFoundException_WhenOfferDoesNotExist()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new RejectOfferCommand(offerId, "reason");

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RejectOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync((Offer?)null);

            // Act
            var act = async () => await _handler.RejectOfferAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RejectOfferAsync_ShouldThrowInvalidOperationException_WhenOfferIsNotPending()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new RejectOfferCommand(offerId, "reason");

            var offer = new Offer(
                "Test Offer",
                "Description",
                Guid.NewGuid(),
                null,
                100m,
                50m,
                10,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(7),
                Guid.NewGuid()
            );

            // Approve it to make status Approved instead of Pending
            offer.Approve();

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RejectOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var act = async () => await _handler.RejectOfferAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Only pending offers can be approved."); 
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RejectOfferAsync_ShouldThrowArgumentException_WhenReasonIsEmpty()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new RejectOfferCommand(offerId, "");

            var offer = new Offer(
                "Test Offer",
                "Description",
                Guid.NewGuid(),
                null,
                100m,
                50m,
                10,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(7),
                Guid.NewGuid()
            );

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RejectOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var act = async () => await _handler.RejectOfferAsync(CancellationToken.None, command).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Rejection reason is required.");
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
