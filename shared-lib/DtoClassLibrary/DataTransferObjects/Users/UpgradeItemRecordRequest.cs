using DtoClassLibrary.DataTransferObjects.Users.Inventory;

namespace DtoClassLibrary.DataTransferObjects.Users
{
    public class UpgradeItemRecordRequest
    {
        public AddRemoveItemRequest? OldItemRecord { get; set; }
        public AddRemoveItemRequest? NewItemRecord { get; set; }
    }
}
