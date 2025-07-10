using DtoClassLibrary.DataTransferObjects.Bonus;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialService.Database.Models.Bonuses;

/// <summary>Бонус на случайный кейс</summary>
public class RandomCaseBonus : Bonus
{
    [Column("minimum_deposit")]
    public decimal MinimumDeposit { get; set; }

    public IBonusDto ConvertToBonusDto()
    {
        return new RandomCaseBonusDto()
        {
            Name = Name,
            Id = Id,
            ImageForDepositView = ImageForDepositView,
            BonusType = BonusType,
            BonusImage = BonusImage,
            Description = Description,
            MinimumDeposit = MinimumDeposit,
            IsDeleted = IsDeleted
        };
    }
}