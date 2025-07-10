
using DtoClassLibrary.DataTransferObjects.CasesItems;

namespace DtoClassLibrary.DataTransferObjects.Users.Inventory;

public class ChangeStateRequest
{
    public List<string> InventoryRecordsIds { get; set; } = new();
    public ItemRecordState ItemRecordState { get; set; }
}

