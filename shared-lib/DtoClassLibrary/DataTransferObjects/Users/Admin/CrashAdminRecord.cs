namespace DtoClassLibrary.DataTransferObjects.Users.Admin;

public class CrashAdminRecord
{
    public string Id { get; set; }
    public decimal Bet { get; set; }
    public bool Won { get; set; }
    public decimal Profit { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }
}