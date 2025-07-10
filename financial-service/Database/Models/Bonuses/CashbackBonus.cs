using System.ComponentModel.DataAnnotations.Schema;
using DtoClassLibrary.DataTransferObjects.Bonus;

namespace FinancialService.Database.Models.Bonuses;

/// <summary>Бонус кэшбэка</summary>
public class CashbackBonus : Bonus
{
    [Column("cashback_percentage")]
    public decimal CashbackPercentage { get; set; }

    [Column("duration")] public TimeSpan Duration { get; set; }

    public IBonusDto ConvertToBonusDto()
    {
        return new CashbackBonusDto()
        {
            Name = Name,
            Id = Id,
            ImageForDepositView = ImageForDepositView,
            BonusType = BonusType,
            BonusImage = BonusImage,
            CashbackPercentage = CashbackPercentage,
            Duration = Duration,
            Description = Description,
            IsDeleted = IsDeleted
        };
    }
}