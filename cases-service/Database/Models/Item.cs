using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace CasesService.Database.Models;

/// <summary>Предмет</summary>
[Table("item")]
public class Item
{
    [Key, Required][Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column("name")] public string? Name { get; set; }

    [Column("type")] public ItemType Type { get; set; }

    [Column("rarity")] public EItemRarity Rarity { get; set; }
    [Column("available_for_upgrade")] public bool IsAvailableForUpgrade { get; set; }

    [Column("base_cost")] public decimal? BaseCost { get; set; }

    [Column("sell_price")] public decimal? SellPrice { get; set; }

    [Column("is_visible")] public bool? IsVisible { get; set; }

    [Column("image")] public string Image { get; set; }

    [Column("items_cases")] public ICollection<ItemCase>? ItemsCases { get; set; }

    [Column("game")] public string? Game { get; set; }

    [Column("is_available_for_contract")] public bool IsAvailableForContract { get; set; } = false;

    public ItemDto CreateDto()
    {
        return new ItemDto()
        {
            Id = Id,
            Name = Name,
            Type = Type,
            Rarity = Rarity,
            BaseCost = BaseCost,
            SellPrice = SellPrice,
            Image = Image,
            Game = Game,
            IsVisible = IsVisible,
        };
    }
}