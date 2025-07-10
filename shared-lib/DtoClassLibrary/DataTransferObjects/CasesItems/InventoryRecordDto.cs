using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace DtoClassLibrary.DataTransferObjects.CasesItems;

public class InventoryRecordDto
{
    public string InventoryRecordId { get; set; }
    public ItemDto Item { get; set; }
    public bool IsItemActive { get; set; }
    public ItemRecordState ItemState { get; set; }
}

