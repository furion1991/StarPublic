
using Newtonsoft.Json;

namespace DtoClassLibrary.DataTransferObjects.Auth.VkAuth;
public class VkUserInfo
{
    [JsonProperty("id")] public long Id { get; set; }
    [JsonProperty("first_name")] public string FirstName { get; set; }
    [JsonProperty("last_name")] public string LastName { get; set; }
    [JsonProperty("photo_200")] public string PhotoUrl { get; set; }
    public string? Email { get; set; }
}

public class VkUserResponse
{
    [JsonProperty("response")] public List<VkUserInfo> Response { get; set; }
}

