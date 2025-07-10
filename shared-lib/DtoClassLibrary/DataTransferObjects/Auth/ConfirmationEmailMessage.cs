namespace DtoClassLibrary.DataTransferObjects.Auth;

public class ConfirmationEmailMessage
{
    public string To { get; set; }
    public string ConfirmationLink { get; set; }
}
