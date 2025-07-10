using DtoClassLibrary.DataTransferObjects.Bonus;

namespace FinancialService.Database.Models.Bonuses
{
    public interface IDtoCreator
    {
        IBonusDto CreateBonusDto();
    }
}
