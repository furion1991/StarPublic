using CasesService.Database.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace CasesService.Services.Converters;

/// <summary>Класс конвертера Item в ItemDto</summary>
public class ItemToDto(Item? item = null) : IConverter<Item, ItemDto>
{
    private readonly Item? _item = item;

    public ItemDto? Convert()
    {
        if (_item == null)
            return null;

        return Convert(_item);
    }

    public ItemDto Convert(Item item)
    {
        return new ItemDto()
        {
            Id = item.Id,
            Name = item.Name,
            Type = item.Type,
            Rarity = item.Rarity,
            BaseCost = item.BaseCost,
            SellPrice = item.SellPrice,
            IsVisible = item.IsVisible,
            Game = item.Game,
            Image = item.Image,
            IsAvailableForContract = item.IsAvailableForContract,
        };
    }

    public List<ItemDto> Convert(List<Item> items)
    {
        List<ItemDto> result = [];
        foreach (Item item in items)
            result.Add(Convert(item));

        return result;
    }
}