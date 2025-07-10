namespace DtoClassLibrary.DataTransferObjects.Auth;

public class ResetPasswordEmailMessage
{
    public string To { get; set; }
    public string NewPassword { get; set; }
}

