using DtoClassLibrary.DataTransferObjects.Bonus;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace FinancialService.Database.Models.Bonuses;

public class LetterBonus : Bonus
{
    [Column("letter")] public string Letter { get; set; }

    public IBonusDto ConvertToBonusDto()
    {
        return new LetterBonusDto()
        {
            Name = Name,
            Id = Id,
            ImageForDepositView = ImageForDepositView,
            BonusType = BonusType,
            BonusImage = BonusImage,
            Description = Description,
            Letter = Letter,
            IsDeleted = IsDeleted
        };
    }
}

