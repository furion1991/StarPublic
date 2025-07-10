namespace DataTransferLib.DataTransferObjects.Financial;

/// <summary>Класс для получения корректного json для работы с финансовыми данными</summary>
public class FinancialDataParams 
{
    public string? UserId { get; set; }
    public decimal? CurrentBalance { get; set; }
}