using DtoClassLibrary.DataTransferObjects.Users.Admin;

namespace DtoClassLibrary.DataTransferObjects.Financial.Withdrawals;

public class WithdrawalListDto
{
    public string Id { get; set; }
    public DateTime Date { get; set; }
    public string? AdminId { get; set; }
    public string? AdminName { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    public decimal ReclaimAmount { get; set; }
    public string? DonateType { get; set; }
    public WithdrawalStatus Status { get; set; }
}