using AutoMapper;
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
        }
    }
}