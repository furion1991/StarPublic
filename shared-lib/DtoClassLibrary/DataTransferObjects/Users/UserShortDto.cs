namespace DtoClassLibrary.DataTransferObjects.Users;

public class UserShortDto
{
    public string? Id { get; set; }
    public string? Username { get; set; }
    public decimal Balance { get; set; }
    public decimal BonusBalance { get; set; }
}