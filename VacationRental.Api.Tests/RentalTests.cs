using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class RentalTests
    {
        private readonly HttpClient _client;

        public RentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
        {
            var request = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = 1
            };

            var postResult = await _client.CreateRentalAndAssertSuccess(request);
            var getResult = await _client.GetRentalAndAssertSuccess(postResult.Id);
            
            Assert.Equal(request.Units, getResult.Units);
            Assert.Equal(request.PreparationTimeInDays, getResult.PreparationTimeInDays);
        }

        [Fact]
        public async Task ChangeRentalWithExistingBookings_BookingsDoesNotOverlap_RentalIsSuccessfullyUpdated()
        {
            var request = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 1
            };

            var createdRental = await _client.CreateRentalAndAssertSuccess(request);
            await _client.CreateBookingAndAssertSuccess(new BookingBindingModel
            {
                RentalId = createdRental.Id,
                Start = new DateTime(2021, 03, 23),
                Nights = 2
            });
            await _client.CreateBookingAndAssertSuccess(new BookingBindingModel()
            {
                RentalId = createdRental.Id,
                Start = new DateTime(2021, 03, 27),
                Nights = 1
            });
            
            var changeRentalRequest = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 2
            };
            
            using (var changeResponse =
                await _client.PutAsJsonAsync($"/api/v1/rentals/{createdRental.Id}", changeRentalRequest))
            {
                Assert.True(changeResponse.IsSuccessStatusCode);
            }

            var rental = await _client.GetRentalAndAssertSuccess(createdRental.Id);
            
            Assert.Equal(changeRentalRequest.Units, rental.Units);
            Assert.Equal(changeRentalRequest.PreparationTimeInDays, rental.PreparationTimeInDays);
        }
        
        [Fact]
        public async Task ChangeRentalWithExistingBookings_BookingsOverlap_BadRequest()
        {
            var request = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 1
            };

            var createdRental = await _client.CreateRentalAndAssertSuccess(request);
            await _client.CreateBookingAndAssertSuccess(new BookingBindingModel
            {
                RentalId = createdRental.Id,
                Start = new DateTime(2021, 03, 23),
                Nights = 2
            });
            await _client.CreateBookingAndAssertSuccess(new BookingBindingModel()
            {
                RentalId = createdRental.Id,
                Start = new DateTime(2021, 03, 24),
                Nights = 1
            });
            
            var changeRentalRequest = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 2
            };
            
            using (var changeResponse =
                await _client.PutAsJsonAsync($"/api/v1/rentals/{createdRental.Id}", changeRentalRequest))
            {
                Assert.Equal(HttpStatusCode.BadRequest, changeResponse.StatusCode);
            }
        }
    }
}
