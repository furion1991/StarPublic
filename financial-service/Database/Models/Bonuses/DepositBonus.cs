using System.ComponentModel.DataAnnotations.Schema;
using DtoClassLibrary.DataTransferObjects.Bonus;

namespace FinancialService.Database.Models.Bonuses;

/// <summary>Бонус к пополнению</summary>
public class DepositBonus : Bonus
{
    [Column("deposit_cap")]
    public decimal DepositCap { get; set; }
    [Column("bonus_multiplier")]
    public decimal BonusMultiplier { get; set; }
    [Column("multiplier_type")]
    public MultiplierType Mtype { get; set; }

    public IBonusDto ConvertToBonusDto()
    {
        return new DepositBonusDto()
        {
            Name = Name,
            Id = Id,
            ImageForDepositView = ImageForDepositView,
            BonusType = BonusType,
            BonusImage = BonusImage,
            Description = Description,
            BonusMultiplier = BonusMultiplier,
            DepositCap = DepositCap,
            IsDeleted = IsDeleted
        };
    }
}