namespace DtoClassLibrary.DataTransferObjects.Users.Admin;

public class WithdrawalDto
{
    public string Id { get; set; }
    public WithdrawalStatus Status { get; set; }
    public ItemRecordDto ItemRecordDto { get; set; }
    public string Message { get; set; }
    public DateTime DateCreated { get; set; }
}

public enum WithdrawalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Transferred = 3
}