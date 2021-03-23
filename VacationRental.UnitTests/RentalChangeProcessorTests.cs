using System.Collections.Generic;
using System.Linq;
using Moq;
using NodaTime;
using NUnit.Framework;
using VacationRental.Common;
using VacationRental.Domain;

namespace VacationRental.UnitTests
{
    public class RentalChangeProcessorTests
    {
        [SetUp]
        public void SetUp()
        {
            _preparationSchedulerMock = new Mock<IPreparationScheduler>();
            // _preparationSchedulerMock
            //     .Setup(scheduler => scheduler.SchedulePreparationAfterBooking(It.IsAny<Rental>(), It.IsAny<Booking>()));
            _rentalAvailabilityCalculatorMock = new Mock<IRentalAvailabilityCalculator>();
            _unitOccupationRepoMock = new Mock<IUnitOccupationRepository>();
            _rentalChangeProcessor = new RentalChangeProcessor(
                _unitOccupationRepoMock.Object,
                _rentalAvailabilityCalculatorMock.Object,
                _preparationSchedulerMock.Object);
            _changedRental = new Rental(2, Period.FromDays(1));
        }
        
        [Test]
        public void RescheduleBookingWithPreparation_AvailabilityCalculatedAsIfTheyAreAbsent()
        {
            var (firstBooking, preparationAfterFirstBooking) =
                CreateBookingWithPreparation(bookingId: 1, unitNumber: 1, preparationId: 2);
            var (secondBooking, preparationAfterSecondBooking) =
                CreateBookingWithPreparation(bookingId: 3, unitNumber: 2, preparationId: 4);
            SetupRepoToReturnOccupations(
                firstBooking, 
                preparationAfterFirstBooking, 
                secondBooking, 
                preparationAfterSecondBooking);
            SetupCalculatorToReturnUnitNumbers(firstBooking.UnitNumber, secondBooking.UnitNumber);
            
            _rentalChangeProcessor.RescheduleOccupationsForNewRentalParameters(_changedRental);

            _rentalAvailabilityCalculatorMock.Verify(
                calculator => calculator.GetAvailableUnitNumbers(
                    _changedRental,
                    It.Is<IEnumerable<UnitOccupation>>(collection =>
                        collection.Count() == 2
                        && collection.Contains(firstBooking) 
                        && collection.Contains(preparationAfterFirstBooking)),
                    secondBooking.StartDate,
                    secondBooking.Nights), Times.Once);
            _rentalAvailabilityCalculatorMock.Verify(
                calculator => calculator.GetAvailableUnitNumbers(
                    _changedRental,
                    It.Is<IEnumerable<UnitOccupation>>(collection =>
                        collection.Count() == 2
                        && collection.Contains(secondBooking) 
                        && collection.Contains(preparationAfterSecondBooking)),
                    firstBooking.StartDate,
                    firstBooking.Nights), Times.Once);
            _rentalAvailabilityCalculatorMock.VerifyNoOtherCalls();
        }

        [Test]
        public void SameRentalUnitIsUnavailable_RentalChangeFailed()
        {
            var (booking, _) =
                CreateBookingWithPreparation(bookingId: 1, unitNumber: 1, preparationId: 2);
            SetupRepoToReturnOccupations(booking);
            SetupCalculatorToReturnUnitNumbers(booking.UnitNumber + 1);

            Assert.Throws<DomainException<RentalChangeFailed>>(() =>
                _rentalChangeProcessor.RescheduleOccupationsForNewRentalParameters(_changedRental));
        }

        [Test]
        public void SameRentalUnitIsAvailable_PreparationTimeIsRescheduled()
        {
            var (booking, preparationAfterBooking) =
                CreateBookingWithPreparation(bookingId: 1, unitNumber: 1, preparationId:2);
            SetupRepoToReturnOccupations(booking, preparationAfterBooking);
            SetupCalculatorToReturnUnitNumbers(booking.UnitNumber);

            _rentalChangeProcessor.RescheduleOccupationsForNewRentalParameters(_changedRental);
            
            _unitOccupationRepoMock.Verify(repo => repo.Remove(preparationAfterBooking.Id), Times.Once);
            _preparationSchedulerMock.Verify(
                scheduler => scheduler.SchedulePreparationAfterBooking(_changedRental, booking), 
                Times.Once);
        }

        private static (Booking booking, Preparation preparation) CreateBookingWithPreparation(
            int bookingId,
            int unitNumber,
            int preparationId)
        {
            var booking = new Booking(
                bookingId, 
                rentalId: 1,
                startDate: new LocalDate(2021, 03, 21), 
                nights: 2,
                unitNumber);
            var preparationAfterBooking = new Preparation(
                preparationId,
                rentalId: 1, 
                startDate: new LocalDate(2021, 03, 23),
                nights: 2, 
                booking.Id, 
                booking.UnitNumber);
            return (booking, preparationAfterBooking);
        }
        
        private void SetupCalculatorToReturnUnitNumbers(params int[] unitNumbers)
        {
            _rentalAvailabilityCalculatorMock
                .Setup(
                    calculator => calculator.GetAvailableUnitNumbers(
                        It.IsAny<Rental>(),
                        It.IsAny<IEnumerable<UnitOccupation>>(),
                        It.IsAny<LocalDate>(),
                        It.IsAny<int>()))
                .Returns(unitNumbers);
        }

        private void SetupRepoToReturnOccupations(params UnitOccupation[] occupations)
        {
            _unitOccupationRepoMock
                .Setup(repo => repo.GetForRental(It.IsAny<int>()))
                .Returns(occupations);
        }
        
        private Mock<IUnitOccupationRepository> _unitOccupationRepoMock;
        private Mock<IPreparationScheduler> _preparationSchedulerMock;
        private Mock<IRentalAvailabilityCalculator> _rentalAvailabilityCalculatorMock;
        private RentalChangeProcessor _rentalChangeProcessor;
        private Rental _changedRental;
    }
}