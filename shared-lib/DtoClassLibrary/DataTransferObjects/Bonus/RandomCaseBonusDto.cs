
namespace DtoClassLibrary.DataTransferObjects.Bonus
{
    public class RandomCaseBonusDto : BonusBaseDto, IBonusDto
    {
        public decimal MinimumDeposit { get; set; }
    }
}
