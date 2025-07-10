using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace UsersService.Models.DbModels
{
    [Table("userinventory")]
    public class UserInventory
    {
        [Key][Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();


        [Column("itemsuserinventoryid")] public ICollection<InventoryItemRecord>? InventoryRecords { get; set; } = new List<InventoryItemRecord>();

        [Column("userid")] public string? UserId { get; set; }
        [JsonIgnore] public User? User { get; set; }
    }
}