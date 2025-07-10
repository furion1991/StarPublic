namespace DataTransferLib.DataTransferObjects.Audit;

public class FinansialLogDto : BaseLogDto
{

    /// <summary>Тип сообщения</summary>
    public enum FTYPE : int
    {
        /// <summary>Пополнение</summary>
        Deposit = 0,
        /// <summary>Снятие</summary>
        Withdraw = 1
    }
    public required string FinancialRecordId { get; set; }
    public required FTYPE FinancialLogType { get; set; }
    public decimal Amount { get; set; }
    public decimal BonusAmount { get; set; }
    public required string UserId { get; set; }
}