namespace UsersService.HttpClient
{
    public class HttpClientService
    {
        private readonly HttpClient _httpClient;

        public HttpClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<object> GetCaseAsync(string caseId)
        {
            var response = await _httpClient.GetAsync($"https://yourapi.com/cases/{caseId}");
            response.EnsureSuccessStatusCode();

            var caseData = await response.Content.ReadAsAsync<object>();
            return caseData;
        }

        public async Task<object> GetItemAsync(string itemId)
        {
            var response = await _httpClient.GetAsync($"https://yourapi.com/items/{itemId}");
            response.EnsureSuccessStatusCode();

            var itemData = await response.Content.ReadAsAsync<object>();
            return itemData;
        }
    }
}
