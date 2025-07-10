namespace DtoClassLibrary.DataTransferObjects.CasesItems.Upgrade;

public class UpgradeItemRequest
{
    public required string UserId { get; set; }
    public required string UserInventoryRecordId { get; set; }
    public required string AttemptedItemId { get; set; }
}