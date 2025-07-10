using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DtoClassLibrary.DataTransferObjects.CasesItems;

namespace UsersService.Models.DbModels
{
    [Table("item_records_t")]
    public class InventoryItemRecord
    {
        [Key, Required][Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
        [Column("user_inventory_id")] public string? UserInventoryId { get; set; }
        [Column("item_id")] public string? ItemId { get; set; }
        [Column("state")] public ItemRecordState ItemRecordState { get; set; }
        [Column("is_item_active")] public bool IsItemActive { get; set; }
        public UserInventory? UserInventory { get; set; }
    }
}