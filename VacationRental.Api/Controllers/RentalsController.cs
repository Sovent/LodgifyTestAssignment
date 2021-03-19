using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Application;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly IMapper _mapper;

        public RentalsController(IRentalService rentalService, IMapper mapper)
        {
            _rentalService = rentalService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public async Task<RentalViewModel> Get(int rentalId)
        {
            var rental = await _rentalService.GetRental(new GetRentalQuery(rentalId));
            var viewModel = _mapper.Map<RentalViewModel>(rental);
            return viewModel;
        }

        [HttpPost]
        public async Task<ResourceIdViewModel> Post(RentalBindingModel model)
        {
            var command = _mapper.Map<CreateRentalCommand>(model);
            var newRentalId = await _rentalService.CreateRental(command);
            var viewModel = new ResourceIdViewModel { Id = newRentalId };
            return viewModel;
        }
    }
}
