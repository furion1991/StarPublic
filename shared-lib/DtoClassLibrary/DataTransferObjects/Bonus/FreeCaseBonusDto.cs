
namespace DtoClassLibrary.DataTransferObjects.Bonus
{
    public class FreeCaseBonusDto : BonusBaseDto, IBonusDto
    {
        public int CaseCount { get; set; }

        public decimal MinimumDeposit { get; set; }
    }
}
