namespace DtoClassLibrary.DataTransferObjects.Bonus
{
    public class BalanceBonusDto : BonusBaseDto, IBonusDto
    {
        public decimal Amount { get; set; }
    }
}
