using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace DtoClassLibrary.DataTransferObjects.Audit;

public class DroppedItemDto
{
    public string CaseId { get; set; }
    public ItemDto? Item { get; set; }
    public DateTime OpenedTimeStamp { get; set; }
    public string UserId { get; set; }
    public decimal? SellPrice { get; set; }
}

