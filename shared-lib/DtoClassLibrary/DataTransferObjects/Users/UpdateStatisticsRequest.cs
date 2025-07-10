namespace DtoClassLibrary.DataTransferObjects.Users;

public class UpdateStatisticsRequest
{
    public string UserId { get; set; } = null!;
    public int AddCasesBought { get; set; } = 0;
    public int AddFailScore { get; set; } = 0;
    public bool ResetFailScore { get; set; } = false;
    public decimal AddSpent { get; set; } = 0;
    public decimal AddProfit { get; set; } = 0;
}