using DtoClassLibrary.DataTransferObjects.Bonus;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialService.Database.Models.Bonuses;

/// <summary>Бонус предметов при пополнении</summary>
public class ItemBonus : Bonus
{
    [Column("item_count")] public int ItemCount { get; set; }
    [Column("minimum_deposit")] public decimal MinimumDeposit { get; set; }
    [Column("item_min_cost")] public decimal ItemMinimalCost { get; set; }
    [Column("item_max_cost")] public decimal ItemMaximalCost { get; set; }
    [Column("is_deposit_dependent")] public bool IsDepositDependent { get; set; }



    public IBonusDto ConvertToBonusDto()
    {
        return new ItemBonusDto()
        {
            Name = Name,
            Id = Id,
            ImageForDepositView = ImageForDepositView,
            BonusType = BonusType,
            BonusImage = BonusImage,
            Description = Description,
            ItemCount = ItemCount,
            MinimumDeposit = MinimumDeposit,
            IsDepositDependent = IsDepositDependent,
            IsDeleted = IsDeleted
        };
    }
}