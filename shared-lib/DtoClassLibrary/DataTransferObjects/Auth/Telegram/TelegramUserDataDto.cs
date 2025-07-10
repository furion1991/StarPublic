using Newtonsoft.Json;

namespace DtoClassLibrary.DataTransferObjects.Auth.Telegram;
public class TelegramUserDataDto
{
    [JsonProperty("id")] public long Id { get; set; }
    [JsonProperty("first_name")] public string? FirstName { get; set; }
    [JsonProperty("last_name")] public string? LastName { get; set; }
    [JsonProperty("username")] public string? Username { get; set; }
    [JsonProperty("photo_url")] public string? PhotoUrl { get; set; }
    [JsonProperty("auth_date")] public long AuthDate { get; set; }
    [JsonProperty("hash")] public string? Hash { get; set; }
}

