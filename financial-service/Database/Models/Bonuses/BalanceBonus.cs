using DtoClassLibrary.DataTransferObjects.Bonus;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialService.Database.Models.Bonuses;

/// <summary>Бонус баланса</summary>
public class BalanceBonus : Bonus
{
    [Column("amount")]
    public decimal Amount { get; set; }

    public IBonusDto ConvertToBonusDto()
    {
        return new BalanceBonusDto()
        {
            Name = Name,
            Id = Id,
            ImageForDepositView = ImageForDepositView,
            BonusType = BonusType,
            BonusImage = BonusImage,
            Description = Description,
            Amount = Amount,
            IsDeleted = IsDeleted
        };
    }
}