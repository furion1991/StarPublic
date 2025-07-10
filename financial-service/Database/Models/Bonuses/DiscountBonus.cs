using DtoClassLibrary.DataTransferObjects.Bonus;
using System.ComponentModel.DataAnnotations.Schema;
using DtoClassLibrary.DataTransferObjects.CasesItems;

namespace FinancialService.Database.Models.Bonuses;

/// <summary>Бонус скидки на покупку</summary>
public class DiscountBonus : Bonus
{
    [Column("discount_percentage")] public decimal DiscountPercentage { get; set; }
    [Column("case_bonus_type_discount")] public ECaseType CaseType { get; set; }

    public IBonusDto ConvertToBonusDto()
    {
        return new DiscountBonusDto()
        {
            Name = Name,
            Id = Id,
            ImageForDepositView = ImageForDepositView,
            BonusType = BonusType,
            BonusImage = BonusImage,
            Description = Description,
            DiscountPercentage = DiscountPercentage,
            IsDeleted = IsDeleted
        };
    }
}