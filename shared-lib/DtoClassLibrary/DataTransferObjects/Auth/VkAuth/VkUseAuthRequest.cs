namespace DtoClassLibrary.DataTransferObjects.Auth.VkAuth;

public class VkUseAuthRequest
{
    public string CodeVerifier { get; set; }
    public string DeviceId { get; set; }
    public string Code { get; set; }
    public string State { get; set; }

}