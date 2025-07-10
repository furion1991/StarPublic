using Newtonsoft.Json;

namespace DtoClassLibrary.DataTransferObjects.Auth.VkAuth;

public class VkAuthResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } 

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("user_id")]
    public long UserId { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}
