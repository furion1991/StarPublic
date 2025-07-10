namespace DtoClassLibrary.DataTransferObjects.Users.Admin;

public class DepositDto
{
    public string Id { get; set; }
    public decimal AmountActual { get; set; }
    public decimal AmountWithBonus { get; set; }
    public string? Promocode { get; set; }
    public string? Message { get; set; }
    public DateTime Date { get; set; }
}