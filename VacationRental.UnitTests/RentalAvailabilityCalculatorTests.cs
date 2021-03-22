using System.Linq;
using NodaTime;
using NUnit.Framework;
using VacationRental.Common;
using VacationRental.Domain;

namespace VacationRental.UnitTests
{
    public class RentalAvailabilityCalculatorTests
    {
        [SetUp]
        public void SetUp()
        {
            _rentalAvailabilityCalculator = new RentalAvailabilityCalculator();
        }

        [Test]
        public void OccupationStartsBeforeRequested_startDate_RepresentedInSchedule()
        {
            var endDate = _startDate.PlusDays(2);
            var booking = new Booking(rentalId: 1, startDate: _startDate.PlusDays(-1), nights: 3, unitNumber: 1);

            var schedule =
                _rentalAvailabilityCalculator.GetOccupationCalendar(new[] {booking}, _startDate, endDate);

            CollectionAssert.Contains(schedule[_startDate], booking);
            CollectionAssert.Contains(schedule[_startDate.PlusDays(1)], booking);
            CollectionAssert.DoesNotContain(schedule[_startDate.PlusDays(2)], booking);
        }

        [Test]
        public void OccupationEndsBeforeRequested_startDate_IsNotRepresentedInSchedule()
        {
            var endDate = _startDate.PlusDays(2);
            var booking = new Booking(rentalId: 1, startDate: _startDate.PlusDays(-3), nights: 1, unitNumber: 1);

            var schedule =
                _rentalAvailabilityCalculator.GetOccupationCalendar(new[] {booking}, _startDate, endDate);

            Assert.IsFalse(schedule.Values.Flatten().Any());
        }

        [Test]
        public void OccupationStartsAfterRequestedEndDate_IsNotRepresentedInSchedule()
        {
            var endDate = _startDate.PlusDays(2);
            var booking = new Booking(rentalId: 1, startDate: endDate.PlusDays(1), nights: 1, unitNumber: 1);

            var schedule =
                _rentalAvailabilityCalculator.GetOccupationCalendar(new[] {booking}, _startDate, endDate);

            Assert.IsFalse(schedule.Values.Flatten().Any());
        }

        [Test]
        public void MultipleOccupationsForOneDay_EachIsPresentInTheSchedule()
        {
            var endDate = _startDate;
            var booking = new Booking(rentalId: 1, startDate: _startDate.PlusDays(-1), nights: 2, unitNumber: 1);
            var anotherBooking = new Booking(rentalId: 1, startDate: _startDate, nights: 1, unitNumber: 2);

            var schedule = _rentalAvailabilityCalculator.GetOccupationCalendar(
                new[] {booking, anotherBooking},
                _startDate,
                endDate);

            CollectionAssert.Contains(schedule[_startDate], booking);
            CollectionAssert.Contains(schedule[_startDate], anotherBooking);
        }

        [Test]
        public void DifferentOccupationsForDifferentDays_AllArePresentInTheSchedule()
        {
            var endDate = _startDate.PlusDays(1);
            var earlyBooking = new Booking(rentalId: 1, startDate: _startDate, nights: 1, unitNumber: 1);
            var latestBooking = new Booking(rentalId: 1, startDate: endDate, nights: 1, unitNumber: 1);

            var schedule = _rentalAvailabilityCalculator.GetOccupationCalendar(
                new[] {earlyBooking, latestBooking},
                _startDate,
                endDate);

            CollectionAssert.Contains(schedule[_startDate], earlyBooking);
            CollectionAssert.DoesNotContain(schedule[_startDate], latestBooking);
            CollectionAssert.Contains(schedule[endDate], latestBooking);
            CollectionAssert.DoesNotContain(schedule[endDate], earlyBooking);
        }

        [Test]
        public void AllUnitsBookedAtOneDay_RentalIsUnavailableForWholePeriod()
        {
            var rental = new Rental(units: 2, Period.Zero);
            var nightsCount = 3;
            var firstUnitBookingForTheDayInTheMiddle = new Booking(
                rentalId: 1,
                startDate: _startDate.PlusDays(1),
                nights: 1,
                unitNumber: 1);
            var secondUnitBookingForTheDayInTheMiddle = new Booking(
                rentalId: 1,
                startDate: _startDate.PlusDays(1),
                nights: 1,
                unitNumber: 2);

            var availableUnitNumbers = _rentalAvailabilityCalculator.GetAvailableUnitNumbers(
                rental,
                new[] {firstUnitBookingForTheDayInTheMiddle, secondUnitBookingForTheDayInTheMiddle},
                _startDate,
                nightsCount);
            
            CollectionAssert.IsEmpty(availableUnitNumbers);
        }

        [Test]
        public void OneUnitIsNotOccupiedForWholePeriod_SecondIsAvailableForPeriod()
        {
            var rental = new Rental(units: 2, Period.Zero);
            var nightsCount = 3;
            var bookingForTheFirstTwoNights = new Booking(rentalId: 1, startDate: _startDate, nights: 2, unitNumber: 1);
            var bookingForTheLastNight =
                new Booking(rentalId: 1, startDate: _startDate.PlusDays(2), nights: 1, unitNumber: 1);

            var availableUnitNumbers = _rentalAvailabilityCalculator.GetAvailableUnitNumbers(
                rental,
                new[] {bookingForTheFirstTwoNights, bookingForTheLastNight},
                _startDate,
                nightsCount);

            Assert.AreEqual(1, availableUnitNumbers.Count);
            CollectionAssert.Contains(availableUnitNumbers, 2);
        }

        [Test]
        public void DifferentUnitAreOccupiedThroughAllegedPeriod_BookingIsUnavailable()
        {
            var rental = new Rental(units: 2, Period.Zero);
            var nightsCount = 3;
            var bookingForTheFirstTwoNights = new Booking(rentalId: 1, startDate: _startDate, nights: 2, unitNumber: 1);
            var bookingForTheLastNight =
                new Booking(rentalId: 1, startDate: _startDate.PlusDays(2), nights: 1, unitNumber: 2);

            var availableUnitNumbers = _rentalAvailabilityCalculator.GetAvailableUnitNumbers(
                rental,
                new[] {bookingForTheFirstTwoNights, bookingForTheLastNight},
                _startDate,
                nightsCount);

            CollectionAssert.IsEmpty(availableUnitNumbers);
        }

        [Test]
        public void AllegedBookingPreparationPeriodOverlapsWithExistingBooking_BookingIsUnavailable()
        {
            var rental = new Rental(units: 1, Period.FromDays(1));
            var nightsCount = 1;
            var bookingForAllegedPreparationPeriod =
                new Booking(rentalId: 1, startDate: _startDate.LastDayAfterSpentNights(2), nights: 1, unitNumber: 1);

            var availableUnitNumbers = _rentalAvailabilityCalculator.GetAvailableUnitNumbers(
                rental,
                new[] {bookingForAllegedPreparationPeriod},
                _startDate,
                nightsCount);
            
            CollectionAssert.IsEmpty(availableUnitNumbers);
        }

        [Test]
        public void AllegedBookingTimeIsOccupiedByPreparation_BookingIsUnavailable()
        {
            var rental = new Rental(units: 1, Period.FromDays(1));
            var nightsCount = 1;
            var preparationForAllegedBookingPeriod =
                new Preparation(rentalId: 1, startDate: _startDate, nights: 1, bookingId: 1, unitNumber: 1);

            var availableUnitNumbers = _rentalAvailabilityCalculator.GetAvailableUnitNumbers(
                rental,
                new[] {preparationForAllegedBookingPeriod},
                _startDate,
                nightsCount);
            
            CollectionAssert.IsEmpty(availableUnitNumbers);
        }

        private RentalAvailabilityCalculator _rentalAvailabilityCalculator;
        private readonly LocalDate _startDate = new LocalDate(2021, 3, 21);
    }
}