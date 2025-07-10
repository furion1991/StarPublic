using DataTransferLib.DataTransferObjects.CasesItems;
using CasesService.Database.Models;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace CasesService.Services.Converters;

/// <summary>Класс конвертера Case в CaseDataDto</summary>
public class CaseToDto(Case? case_ = null) : IConverter<Case, CaseDto>
{
    private readonly Case? _case = case_;

    public CaseDto? Convert()
    {
        if (_case == null)
            return null;

        return Convert(_case);
    }

    public CaseDto Convert(Case case_)
    {
        var newCase = new CaseDto()
        {
            Id = case_.Id,
            Name = case_.Name,
            Image = case_.Image,
            Type = case_.Type,
            Price = case_.Price,
            OpenLimit = case_.OpenLimit,
            Discount = case_.Discount,
            OldPrice = case_.OldPrice,
            CurrentOpen = case_.CurrentOpen,
        };

        newCase.Items = new List<ItemDto>();

        if (case_.ItemsCases is not null)
        {
            foreach (var item in case_.ItemsCases)
            {
                var newItem = new ItemDto()
                {
                    Id = item.ItemId,
                    Type = item.Item.Type,
                    BaseCost = item.Item.BaseCost,
                    Game = item.Item.Game,
                    IsVisible = item.Item.IsVisible,
                    Name = item.Item.Name,
                    Rarity = item.Item.Rarity,
                    SellPrice = item.Item.SellPrice,
                    Image = item.Item.Image,
                    IsAvailableForContract = item.Item.IsAvailableForContract,
                    IsAvailableForUpgrade = item.Item.IsAvailableForUpgrade,
                };
                newCase.Items.Add(newItem);
            }
        }

        return newCase;
    }

    public List<CaseDto> Convert(List<Case> cases)
    {
        List<CaseDto> result = [];
        foreach (Case case_ in cases)
            result.Add(Convert(case_));

        return result;
    }
}