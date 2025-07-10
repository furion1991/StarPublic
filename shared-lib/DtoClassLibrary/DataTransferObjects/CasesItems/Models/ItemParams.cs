namespace DtoClassLibrary.DataTransferObjects.CasesItems.Models;

public class ItemParams
{
    public string? Name { get; set; }
    public ItemType Type { get; set; }
    public EItemRarity Rarity { get; set; }
    public decimal? BaseCost { get; set; }
    public decimal? SellPrice { get; set; }
    public bool IsVisible { get; set; }
    public string? Game { get; set; }
}