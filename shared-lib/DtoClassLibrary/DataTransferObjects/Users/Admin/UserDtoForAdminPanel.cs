namespace DtoClassLibrary.DataTransferObjects.Users.Admin;

public class UserDtoForAdminPanel
{
    public string Id { get; set; }
    public string Username { get; set; }
    public decimal Deposited { get; set; }
    public decimal Profit { get; set; }
    public decimal Balance { get; set; }
    public string Role { get; set; }
    public string SingupDate { get; set; }
}