namespace DtoClassLibrary.DataTransferObjects.Audit.Dashboard;

public class DashboardDto
{
    public CasesOpenDto? CasesOpened { get; set; }
    public DashboardBalanceDto? UsersBalanceData { get; set; }
    public MajorDepositDto? MajorDepositDto { get; set; }
    public UsersDashBoardData? UsersDashBoardData { get; set; }
    public UpgradeDashboardData? UpgradeData { get; set; }
    public CrashDashboardData? CrashData { get; set; }
    public ContractDashboardData? ContractData { get; set; }
}
