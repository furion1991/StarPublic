namespace DtoClassLibrary.DataTransferObjects.Bonus.SmallBonuses;

public class PrizeDrawDto
{
    public decimal CurrentPrizeAmount { get; set; }
    public string? LastWinnerImageUrl { get; set; }
    public string? LastWinnerUserId { get; set; }
    public decimal LastWonPrizeAmount { get; set; }
    public decimal NextPrizeAmount { get; set; }
    public int SecondsRemainingTillDraw { get; set; }
    public int SubscribedUsers { get; set; }
}