using Newtonsoft.Json;

namespace AuthService.Services;

public class TelegramSubscriptionChecker(ILogger<TelegramSubscriptionChecker> logger)
{
    public async Task<bool> CheckUserSubscribedAsync(long telegramUserId, string channelUsername)
    {
        var httpClient = new HttpClient();
        string botToken = Environment.GetEnvironmentVariable("TelegramBotToken") ?? "";
        var url = $"https://api.telegram.org/bot{botToken}/getChatMember" +
                  $"?chat_id=@{channelUsername}" +
                  $"&user_id={telegramUserId}";

        try
        {
            var response = await httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            logger.LogInformation($"📡 Telegram getChatMember response: {json}");

            var result = JsonConvert.DeserializeObject<GetChatMemberResponse>(json);

            return result?.Ok == true && result.Result.Status is "member" or "administrator" or "creator";
        }
        catch (Exception ex)
        {
            logger.LogError($"❌ Telegram subscription check failed: {ex.Message}");
            return false;
        }
    }
}

public class GetChatMemberResponse
{
    [JsonProperty("ok")] public bool Ok { get; set; }

    [JsonProperty("result")] public ChatMemberResult Result { get; set; }
}

public class ChatMemberResult
{
    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("user")] public TelegramUserInfo User { get; set; }
}

public class TelegramUserInfo
{
    [JsonProperty("id")] public long Id { get; set; }

    [JsonProperty("first_name")] public string FirstName { get; set; }

    [JsonProperty("username")] public string Username { get; set; }
}