using DtoClassLibrary.DataTransferObjects.CasesItems;

namespace DtoClassLibrary.DataTransferObjects.Users.Inventory
{
    public class AddRemoveItemRequest
    {
        public string? UserId { get; set; }
        public string? ItemInventoryRecordId { get; set; }
        public int Quantity { get; set; }
        public ItemRecordState ItemRecordState { get; set; }
        public string ItemImageUrl { get; set; }
    }

    public class AddRemoveMultipleItemsRequest
    {
        public string? UserId { get; set; }
        public List<string>? Items { get; set; }
        public ItemRecordState ItemRecordState { get; set; }
    }
}
