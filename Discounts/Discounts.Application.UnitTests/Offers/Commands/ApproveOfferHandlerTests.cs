using Discounts.Application.Common.Exceptions;
using Discounts.Application.Offers.Commands.ApproveOffer;
using Discounts.Application.Offers.Interfaces;
using Discounts.Domain.Offers;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Discounts.Application.UnitTests.Offers.Commands
{
    public class ApproveOfferHandlerTests
    {
        private readonly Mock<IOfferRepository> _repositoryMock;
        private readonly Mock<IValidator<ApproveOfferCommand>> _validatorMock;
        private readonly ApproveOfferHandler _handler;

        public ApproveOfferHandlerTests()
        {
            _repositoryMock = new Mock<IOfferRepository>();
            _validatorMock = new Mock<IValidator<ApproveOfferCommand>>();

            _handler = new ApproveOfferHandler(_repositoryMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task ApproveOfferAsync_ShouldChangeStatusToApproved_WhenOfferExistsAndIsPending()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new ApproveOfferCommand(offerId);

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
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<ApproveOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            offer.Status.Should().Be(OfferStatus.Approved);
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ApproveOfferAsync_ShouldThrowValidationException_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new ApproveOfferCommand(Guid.Empty);
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Id", "Id is required") };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<ApproveOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationFailures));

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            _repositoryMock.Verify(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), It.IsAny<Guid>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ApproveOfferAsync_ShouldThrowNotFoundException_WhenOfferDoesNotExist()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new ApproveOfferCommand(offerId);

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<ApproveOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync((Offer?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ApproveOfferAsync_ShouldThrowInvalidOperationException_WhenOfferIsNotPending()
        {
            // Arrange
            var offerId = Guid.NewGuid();
            var command = new ApproveOfferCommand(offerId);

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
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<ApproveOfferCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetOfferForUpdateByIdAsync(It.IsAny<CancellationToken>(), offerId))
                .ReturnsAsync(offer);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Only pending offers can be approved.");
            _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
