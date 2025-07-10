using DtoClassLibrary.DataTransferObjects.Bonus;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialService.Database.Models.Bonuses;

/// <summary>Бонус для барабана</summary>
public class WheelSpinBonus : Bonus
{
    [Column("extra_spins")]
    public int ExtraSpins { get; set; }


    public IBonusDto ConvertToBonusDto()
    {
        return new WheelSpinBonusDto()
        {
            Name = Name,
            Id = Id,
            ImageForDepositView = ImageForDepositView,
            BonusType = BonusType,
            BonusImage = BonusImage,
            Description = Description,
            ExtraSpins = ExtraSpins,
            IsDeleted = IsDeleted
        };
    }
}