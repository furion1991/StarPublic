using DataTransferLib.DataTransferObjects.CasesItems;

namespace DtoClassLibrary.DataTransferObjects.CasesItems.Models;

/// <summary>Класс для получения корректного json для работы с кейсом</summary>
public class CaseParams
{
    public string? Name { get; set; } = string.Empty;
    public string? Image { get; set; } = string.Empty;
    public ECaseType Type { get; set; } = ECaseType.None;
    public decimal? Price { get; set; } = 0;
    public string? CaseCategory { get; set; }
    public int? OpenLimit { get; set; } = 0;
    public float? Discount { get; set; } = 0;
    public decimal? OldPrice { get; set; } = 0;
    public float Alpha { get; set; } = 0; //todo make remake the library
}