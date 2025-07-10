using DtoClassLibrary.DataTransferObjects.Auth.VkAuth;
using Newtonsoft.Json;

namespace AuthService.Services;

public class VkService(ILogger<VkService> logger)
{
    public async Task<VkAuthResponse> ExchangeCodeForVkToken(string code, string codeVerifier, string deviceId,
        string state, string? redirect = null)
    {
        string clientId = "53082466"; // 🔹 Укажи свой client_id
        string redirectUri = !string.IsNullOrEmpty(redirect)
            ? $"https://front.dev.stardrop.app/{redirect}/"
            : "https://front.dev.stardrop.app/";

        string tokenUrl = "https://id.vk.com/oauth2/auth";

        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "code_verifier", codeVerifier },
            { "redirect_uri", redirectUri },
            { "client_id", clientId },
            { "device_id", deviceId },
            { "state", state },
        };

        logger.LogInformation("📤 VK token request:");
        foreach (var kv in requestBody)
        {
            logger.LogInformation($"{kv.Key} - {kv.Value}");
        }


        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        // 🔥 ВАЖНО: Указываем заголовок явно
        request.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

        var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Ответ VK: {responseBody}");

        return JsonConvert.DeserializeObject<VkAuthResponse>(responseBody) ?? new VkAuthResponse();
    }

    public async Task<bool> CheckGroupSubscriptionAsync(string accessToken, long userId, string groupId)
    {
        var url = $"https://api.vk.com/method/groups.isMember" +
                  $"?group_id={groupId}" +
                  $"&user_id={userId}" +
                  $"&access_token={accessToken}" +
                  $"&v=5.131";

        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            logger.LogInformation($"📡 VK group subscription check URL: {url}");
            logger.LogInformation($"📥 VK response: {json}");

            var vkResponse = JsonConvert.DeserializeObject<VkSubscriptionCheckResponse>(json);

            return vkResponse?.IsMember == 1;
        }
        catch (Exception ex)
        {
            logger.LogError($"❌ Failed to check VK subscription: {ex.Message}");
            return false;
        }
    }

    public async Task<VkRefreshTokenResponse> RefreshVkAccessToken(string refreshToken)
    {
        string clientId = Environment.GetEnvironmentVariable("VK_CLIENT_ID");
        string tokenUrl = "https://id.vk.com/oauth2/auth";

        var requestBody = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("client_id", clientId)
        ]);

        using var client = new HttpClient();
        var response = await client.PostAsync(tokenUrl, requestBody);
        var responseBody = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<VkRefreshTokenResponse>(responseBody) ?? new VkRefreshTokenResponse();
    }


    public async Task<VkUserInfo> GetVkUserInfo(string accessToken, long userId)
    {
        var userInfoUrl =
            $"https://api.vk.com/method/users.get?user_ids={userId}&fields=photo_200&access_token={accessToken}&v=5.131";

        using var client = new HttpClient();
        var response = await client.GetAsync(userInfoUrl);
        var json = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"VK user info request URL: {userInfoUrl}");
        Console.WriteLine($"VK user info raw response: {json}");

        try
        {
            var parsed = JsonConvert.DeserializeObject<VkUserResponse>(json);
            if (parsed?.Response == null)
            {
                Console.WriteLine("VK response did not contain 'response' field or it was null.");
                return new VkUserInfo() { Id = 0 };
            }

            return parsed.Response.FirstOrDefault() ?? new VkUserInfo() { Id = 0 };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while deserializing VK user info: {ex.Message}");
            return new VkUserInfo() { Id = 0 };
        }
    }
}

public class VkSubscriptionCheckResponse
{
    [JsonProperty("response")] public int IsMember { get; set; }
}

public class VkSubscriptionStatus
{
    [JsonProperty("member")] public int IsMember { get; set; } // 1 — подписан, 0 — нет
}