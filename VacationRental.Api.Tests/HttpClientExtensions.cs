using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    public static class HttpClientExtensions
    {
        public static async Task<ResourceIdViewModel> CreateRentalAndAssertSuccess(
            this HttpClient client,
            RentalBindingModel model)
        {
            using (var postResponse = await client.PostAsJsonAsync($"/api/v1/rentals", model))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                return await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }
        }

        public static async Task<ResourceIdViewModel> CreateBookingAndAssertSuccess(
            this HttpClient client,
            BookingBindingModel model)
        {
            using (var postBookingResponse = await client.PostAsJsonAsync($"/api/v1/bookings", model))
            {
                Assert.True(postBookingResponse.IsSuccessStatusCode);
                return await postBookingResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }
        }

        public static async Task<RentalViewModel> GetRentalAndAssertSuccess(this HttpClient client, int rentalId)
        {
            using (var getResponse = await client.GetAsync($"/api/v1/rentals/{rentalId}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);
                return await getResponse.Content.ReadAsAsync<RentalViewModel>();
            }
        }
    }
}