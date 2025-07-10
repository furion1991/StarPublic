namespace DtoClassLibrary.DataTransferObjects.Users.Admin;

public class OpenedCaseAdminDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal CaseCost { get; set; }
    public decimal ItemCost { get; set; }
    public decimal Profit { get; set; }
    public DateTime OpenDate { get; set; }
}