using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataTransferLib.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems;

namespace CasesService.Database.Models;

/// <summary>Кейс</summary>
[Table("case")]
public class Case
{
    [Key, Required][Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column("name")] public string? Name { get; set; }

    [Column("image")] public string? Image { get; set; }

    [Column("type")] public ECaseType Type { get; set; }

    [Column("price")] public decimal? Price { get; set; }

    [Column("current_open")] public int CurrentOpen { get; set; }

    [Column("open_limit")] public int? OpenLimit { get; set; }

    [Column("items_cases")] public ICollection<ItemCase>? ItemsCases { get; set; }

    [Column("discount")] public float? Discount { get; set; }

    [Column("old_price")] public decimal? OldPrice { get; set; }

    [Column("accumulated_profit")] public decimal? AccumulatedProfit { get; set; }
    [Column("is_visible")] public bool IsVisible { get; set; } = true;

    [Column("alpha")]
    public float Alpha { get; set; } = 1.0f;

    [Column("bonus_new_user_enabled")]
    public bool BonusNewUserEnabled { get; set; } = false;

    [Column("bonus_new_user_rolls")]
    public int BonusNewUserRolls { get; set; } = 3;

    public string? CategoryId { get; set; }
    public CaseCategory? CaseCategory { get; set; }

    public void AddAccumulatedProfit()
    {
        AccumulatedProfit += Price;
    }
}