using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class GetCalendarTests
    {
        private readonly HttpClient _client;

        public GetCalendarTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar()
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 1
            };

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };

            ResourceIdViewModel postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 03)
            };

            ResourceIdViewModel postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking2Result = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getCalendarResponse =
                await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights=5"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();

                Assert.Equal(postRentalResult.Id, getCalendarResult.RentalId);
                Assert.Equal(5, getCalendarResult.Dates.Count);

                var january1Calendar = getCalendarResult.Dates[0];
                Assert.Equal(new DateTime(2000, 01, 01), january1Calendar.Date);
                Assert.Empty(january1Calendar.Bookings);
                Assert.Empty(january1Calendar.PreparationTimes);

                var january2Calendar = getCalendarResult.Dates[1];
                Assert.Equal(new DateTime(2000, 01, 02), january2Calendar.Date);
                Assert.Single(january2Calendar.Bookings);
                Assert.Contains(january2Calendar.Bookings, x => x.Id == postBooking1Result.Id && x.Unit == 1);
                Assert.Empty(january2Calendar.PreparationTimes);

                var january3Calendar = getCalendarResult.Dates[2];
                Assert.Equal(new DateTime(2000, 01, 03), january3Calendar.Date);
                Assert.Equal(2, january3Calendar.Bookings.Count);
                Assert.Contains(january3Calendar.Bookings, x => x.Id == postBooking1Result.Id && x.Unit == 1);
                Assert.Contains(january3Calendar.Bookings, x => x.Id == postBooking2Result.Id && x.Unit == 2);
                Assert.Empty(january3Calendar.PreparationTimes);

                var january4Calendar = getCalendarResult.Dates[3];
                Assert.Equal(new DateTime(2000, 01, 04), january4Calendar.Date);
                Assert.Single(january4Calendar.Bookings);
                Assert.Contains(january4Calendar.Bookings, x => x.Id == postBooking2Result.Id && x.Unit == 2);
                Assert.Contains(january4Calendar.PreparationTimes, x => x.Unit == 1);

                var january5Calendar = getCalendarResult.Dates[4];
                Assert.Equal(new DateTime(2000, 01, 05), january5Calendar.Date);
                Assert.Empty(january5Calendar.Bookings);
                Assert.Contains(january5Calendar.PreparationTimes, x => x.Unit == 2);
            }
        }
    }
}