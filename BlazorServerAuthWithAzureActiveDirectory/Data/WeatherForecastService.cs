using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorServerAuthWithAzureActiveDirectory.Data
{
    public class WeatherForecastService
    {
        private readonly TokenProvider _store;

        public WeatherForecastService(IHttpClientFactory clientFactory, TokenProvider tokenProvider)
        {
            Client = clientFactory.CreateClient();
            _store = tokenProvider;
        }

        public HttpClient Client { get; }

        public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var token = _store.AccessToken;
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5003/WeatherForecast");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<WeatherForecast[]>();
        }
    }
}
