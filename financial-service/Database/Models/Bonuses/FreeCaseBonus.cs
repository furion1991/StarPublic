using DtoClassLibrary.DataTransferObjects.Bonus;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialService.Database.Models.Bonuses;

/// <summary>Бонус бесплатных кейсов при пополнении</summary>
public class FreeCaseBonus : Bonus
{
    [Column("case_count")]
    public int CaseCount { get; set; }

    [Column("minimum_deposit")]
    public decimal MinimumDeposit { get; set; }


    public IBonusDto ConvertToBonusDto()
    {
        return new FreeCaseBonusDto()
        {
            Name = Name,
            Id = Id,
            ImageForDepositView = ImageForDepositView,
            BonusType = BonusType,
            BonusImage = BonusImage,
            Description = Description,
            CaseCount = CaseCount,
            MinimumDeposit = MinimumDeposit,
            IsDeleted = IsDeleted
        };
    }
}