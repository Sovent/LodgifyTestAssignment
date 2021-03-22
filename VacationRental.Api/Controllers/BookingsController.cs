using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Application;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public BookingsController(IBookingService bookingService, IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            var booking = _bookingService.GetBooking(new GetBookingQuery(bookingId));
            var viewModel = _mapper.Map<BookingViewModel>(booking);
            return viewModel;
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            var command = _mapper.Map<PlaceBookingCommand>(model);
            var createdBookingId = _bookingService.PlaceBooking(command);
            return new ResourceIdViewModel {Id = createdBookingId};
        }
    }
}
