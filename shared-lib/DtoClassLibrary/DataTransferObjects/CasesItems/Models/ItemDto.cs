using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;

namespace DtoClassLibrary.DataTransferObjects.CasesItems.Models;

public class ItemDto : IDefaultDto
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public ItemType Type { get; set; }
    public EItemRarity Rarity { get; set; }
    public decimal? BaseCost { get; set; }
    public decimal? SellPrice { get; set; }
    public bool? IsVisible { get; set; }
    public string? Game { get; set; }
    public string Image { get; set; }
    public bool IsAvailableForContract { get; set; }
    public bool IsAvailableForUpgrade { get; set; }
}