using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace DtoClassLibrary.DataTransferObjects.Users.Admin;

public class ContractAdminRecordDto
{
    public string Id { get; set; }
    public List<ItemRecordDto> ItemsSpent { get; set; } = [];
    public decimal ItemsAmount { get; set; }
    public ItemDto ItemWon { get; set; }
    public decimal Profit { get; set; }
    public DateTime Date { get; set; }
    public string Message { get; set; } = "";
}