using AutoMapper;
using NodaTime;
using VacationRental.Api.Models;
using VacationRental.Application;
using VacationRental.Domain;

namespace VacationRental.Api
{
    public class ModelMappingProfile : Profile
    {
        public ModelMappingProfile()
        {
            CreateMap<RentalBindingModel, CreateRentalCommand>();
            CreateMap<Rental, RentalViewModel>();
            CreateMap<BookingBindingModel, PlaceBookingCommand>()
                .ForCtorParam(
                    "startDate",
                    options => options.MapFrom(model => LocalDate.FromDateTime(model.Start)));
            CreateMap<Booking, BookingViewModel>()
                .ForMember(
                    model => model.Start,
                    options => options.MapFrom(entity => entity.StartDate.ToDateTimeUnspecified()));
        }
    }
}