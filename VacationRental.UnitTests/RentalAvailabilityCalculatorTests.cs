using System;
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
            var booking = new Booking(rentalId: 1, startDate: _startDate.PlusDays(-1), nights: 3);

            var schedule =
                _rentalAvailabilityCalculator.GetOccupationSchedule(MakeOccupations(booking), _startDate, endDate);
            
            CollectionAssert.Contains(schedule[_startDate], booking);
            CollectionAssert.Contains(schedule[_startDate.PlusDays(1)], booking);
            CollectionAssert.DoesNotContain(schedule[_startDate.PlusDays(2)], booking);
        }

        [Test]
        public void OccupationEndsBeforeRequested_startDate_IsNotRepresentedInSchedule()
        {
            var endDate = _startDate.PlusDays(2);
            var booking = new Booking(rentalId: 1, startDate: _startDate.PlusDays(-3), nights: 1);

            var schedule =
                _rentalAvailabilityCalculator.GetOccupationSchedule(MakeOccupations(booking), _startDate, endDate);

            Assert.IsFalse(schedule.Values.Flatten().Any());
        }

        [Test]
        public void OccupationStartsAfterRequestedEndDate_IsNotRepresentedInSchedule()
        {
            var endDate = _startDate.PlusDays(2);
            var booking = new Booking(rentalId: 1, startDate: endDate.PlusDays(1), nights: 1);

            var schedule =
                _rentalAvailabilityCalculator.GetOccupationSchedule(MakeOccupations(booking), _startDate, endDate);

            Assert.IsFalse(schedule.Values.Flatten().Any());
        }
        
        [Test]
        public void MultipleOccupationsForOneDay_EachIsPresentInTheSchedule()
        {
            var endDate = _startDate;
            var booking = new Booking(rentalId: 1, startDate: _startDate.PlusDays(-1), nights: 2);
            var anotherBooking = new Booking(rentalId: 1, startDate: _startDate, nights: 1);

            var schedule = _rentalAvailabilityCalculator.GetOccupationSchedule(
                MakeOccupations(booking, anotherBooking),
                _startDate,
                endDate);

            CollectionAssert.Contains(schedule[_startDate], booking);
            CollectionAssert.Contains(schedule[_startDate], anotherBooking);
        }

        [Test]
        public void DifferentOccupationsForDifferentDays_AllArePresentInTheSchedule()
        {
            var endDate = _startDate.PlusDays(1);
            var earlyBooking = new Booking(rentalId: 1, startDate: _startDate, nights: 1);
            var latestBooking = new Booking(rentalId: 1, startDate: endDate, nights: 1);

            var schedule = _rentalAvailabilityCalculator.GetOccupationSchedule(
                MakeOccupations(earlyBooking, latestBooking),
                _startDate,
                endDate);

            CollectionAssert.Contains(schedule[_startDate], earlyBooking);
            CollectionAssert.DoesNotContain(schedule[_startDate], latestBooking);
            CollectionAssert.Contains(schedule[endDate], latestBooking);
            CollectionAssert.DoesNotContain(schedule[endDate], earlyBooking);
        }

        [Test]
        public void AllUnitsOccupiedAtOneDay_RentalIsUnavailableForWholePeriod()
        {
            var rental = new Rental(units: 2);
            var nightsCount = 2;
            var bookingForTheDayInTheMiddle = new Booking(rentalId: 1, startDate: _startDate.PlusDays(1), nights: 1);

            Assert.Throws<DomainException<RentalIsUnavailable>>(
                () => _rentalAvailabilityCalculator.CheckAvailability(
                    rental,
                    MakeOccupations(bookingForTheDayInTheMiddle, bookingForTheDayInTheMiddle),
                    _startDate,
                    nightsCount));
        }

        [Test]
        public void EveryDayOfPeriodHasFreeUnit_RentalIsAvailableForPeriod()
        {
            var rental = new Rental(units: 2);
            var nightsCount = 3;
            var bookingForTheFirstTwoNights = new Booking(rentalId: 1, startDate: _startDate, nights: 2);
            var bookingForTheLastNight = new Booking(rentalId: 1, startDate: _startDate.LastDayAfterSpentNights(3), nights: 2);

            _rentalAvailabilityCalculator.CheckAvailability(
                rental,
                MakeOccupations(bookingForTheFirstTwoNights, bookingForTheLastNight), 
                _startDate, 
                nightsCount);
            
            // assert no exception
        }

        private static IQueryable<UnitOccupation> MakeOccupations(params UnitOccupation[] occupations)
        {
            return occupations.AsQueryable();
        }
        
        private RentalAvailabilityCalculator _rentalAvailabilityCalculator;
        private readonly LocalDate _startDate = new LocalDate(2021, 3, 21);
    }
}