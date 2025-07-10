using DtoClassLibrary.DataTransferObjects.Bonus;

namespace FinancialService.Database.Models.Bonuses
{
    public class FiveKBonus : Bonus
    {

        public IBonusDto ConvertToBonusDto()
        {
            return new FiveKBonusDto()
            {
                Name = Name,
                Id = Id,
                ImageForDepositView = ImageForDepositView,
                BonusType = BonusType,
                BonusImage = BonusImage,
                Description = Description,
                IsDeleted = IsDeleted
            };
        }
    }
}
