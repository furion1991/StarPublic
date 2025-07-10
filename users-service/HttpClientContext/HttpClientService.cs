using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

namespace UsersService.HttpClientContext
{
    public class HttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientService(IHttpClientFactory httpClient)
        {
            _httpClientFactory = httpClient;
        }

        public async Task<string> GetItemAsync(string itemId)
        {
            var client = _httpClientFactory.CreateClient("auth");

            var response = await client.GetAsync($"auth/{itemId}");

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
