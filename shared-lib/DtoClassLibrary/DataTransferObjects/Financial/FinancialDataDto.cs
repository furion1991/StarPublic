using DataTransferLib.DataTransferObjects.Common.Interfaces;

namespace DataTransferLib.DataTransferObjects.Financial;

/// <summary>Класс для корректной работы с объектом финансовых данных и их сущностью в БД</summary>
public class FinancialDataDto : IDefaultDto
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public decimal CurrentBalance { get; set; }
}