namespace DtoClassLibrary.DataTransferObjects.Auth;
public class SocialLoginRequest
{
    public string Provider { get; set; }
    public string? Data { get; set; }
    public VkData? VkData { get; set; }

}

public class VkData
{
    public string State { get; set; }
    public string DeviceId { get; set; }
    public string CodeVerifier { get; set; }
    public string Code { get; set; }
}
