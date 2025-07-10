using DataTransferLib.DataTransferObjects.CasesItems;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace DtoClassLibrary.DataTransferObjects.CasesItems;

public class CaseDto : IDefaultDto
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
    public int CurrentOpen { get; set; }
    public ECaseType Type { get; set; }
    public decimal? Price { get; set; }
    public int? OpenLimit { get; set; }
    public float? Discount { get; set; }
    public decimal? OldPrice { get; set; }
    public string CaseCategory { get; set; }
    public ICollection<ItemDto> Items { get; set; }
}

public class CaseListDto : IDefaultDto
{
    public List<CaseDto> Cases { get; set; }
    public int Count { get; set; }
}