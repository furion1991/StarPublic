namespace DtoClassLibrary.DataTransferObjects.Financial.Withdrawals;

public class WithdrawalDetails
{
    public WithdrawalListDto Withdrawal { get; set; }
    public WithdrawalContact WithdrawalContact { get; set; }
    public List<string> ItemIds { get; set; } = [];
}