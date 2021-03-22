using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using VacationRental.Api.Models;
using VacationRental.Application;
using VacationRental.Common;
using VacationRental.Domain;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        public CalendarController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            var startDate = LocalDate.FromDateTime(start);
            var endDate = startDate.LastDayAfterSpentNights(nights);
            var query = new GetBookingCalendarQuery(rentalId, startDate, endDate);
            var calendar = _bookingService.GetBookingCalendar(query);

            var dateViewModels = calendar.Keys.Select(date => new CalendarDateViewModel
            {
                Date = date.ToDateTimeUnspecified(),
                Bookings = calendar[date]
                    .OfType<Booking>()
                    .Select(booking => new CalendarBookingViewModel {Id = booking.Id})
                    .ToList()
            }).ToList();

            return new CalendarViewModel()
            {
                RentalId = rentalId,
                Dates = dateViewModels.ToList()
            };
        }
        
        private readonly IBookingService _bookingService;
    }
}
