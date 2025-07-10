using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace DtoClassLibrary.DataTransferObjects.CasesItems;

public class GameResultDto
{
    public string UserId { get; set; }
    public List<InventoryRecordDto> Items { get; set; } = new List<InventoryRecordDto>();
}