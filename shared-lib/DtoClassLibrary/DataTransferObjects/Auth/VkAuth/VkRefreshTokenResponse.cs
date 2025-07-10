using Newtonsoft.Json;

namespace DtoClassLibrary.DataTransferObjects.Auth.VkAuth;

public class VkRefreshTokenResponse
{
    [JsonProperty("access_token")] public string AccessToken { get; set; }
    [JsonProperty("expires_in")] public int ExpiresIn { get; set; }

}