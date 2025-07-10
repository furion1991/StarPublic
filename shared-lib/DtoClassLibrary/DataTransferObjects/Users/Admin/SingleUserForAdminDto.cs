namespace DtoClassLibrary.DataTransferObjects.Users.Admin;

public class SingleUserForAdminDto
{
    public string UserId { get; set; }
    public string Role { get; set; }
    public decimal AddedByAdmin { get; set; }
    public decimal Balance { get; set; }
    public decimal Deposited { get; set; }
    public decimal Profit { get; set; }
    public decimal BonusBalance { get; set; }
    public bool IsBlocked { get; set; }
    public List<OpenedCaseAdminDto> OpenedCases { get; set; } = [];
    public List<WithdrawalDto> Withdrawals { get; set; } = [];
    public List<DepositDto> Deposits { get; set; } = [];
    public List<ContractAdminRecordDto> Contracts { get; set; } = [];
    public List<UpgradeAdminRecordDto> Upgrades { get; set; } = [];
}
