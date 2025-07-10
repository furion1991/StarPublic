namespace DataTransferLib.DataTransferObjects.Common;

public class Log
{
    public required string Message { get; set; }
    public required object Content { get; set; }
    public required LTYPE LogType { get; set; }
    public string? PerformedById { get; set; }
    public string? ObjectId { get; set; }
    public int? Type { get; set; }
}


public enum LTYPE : int
{
    Base,
    Case,
    Financial,
    Item,
    User,
    Deposit,
    Login,
    CaseOpened,
    Withdrawal,
    Upgrade,
    UserRegister,
    Contract
}
