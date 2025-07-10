namespace CasesService.Controllers.Models;

/// <summary>Класс для получения корректного json для работы с предметом кейса</summary>
public class ItemCaseParams
{
    public string? CaseId { get; set; }
    public string? ItemId { get; set; }
}