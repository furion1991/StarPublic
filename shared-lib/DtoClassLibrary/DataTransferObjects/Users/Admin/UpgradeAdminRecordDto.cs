using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace DtoClassLibrary.DataTransferObjects.Users.Admin;

public class UpgradeAdminRecordDto
{
    public string Id { get; set; }
    public ItemRecordDto ItemSpent { get; set; }
    public ItemDto ItemAttempted { get; set; }
    public bool Won { get; set; }
    public decimal Chance { get; set; }
    public decimal Profit { get; set; }
    public DateTime Date { get; set; }
}