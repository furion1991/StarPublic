
namespace DtoClassLibrary.DataTransferObjects.Bonus
{
    public class CashbackBonusDto : BonusBaseDto, IBonusDto
    {
        public decimal CashbackPercentage { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
