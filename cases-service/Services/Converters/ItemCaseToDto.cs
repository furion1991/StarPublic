using CasesService.Database.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DataTransferLib.DataTransferObjects.CasesItems;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace CasesService.Services.Converters;

/// <summary>Класс конвертера ItemCase в ItemCaseDto</summary>
public class ItemCaseToDto(ItemCase? itemCase = null) : IConverter<ItemCase, ItemCaseDto>
{
    private readonly ItemCase? _itemCase = itemCase;

    public ItemCaseDto? Convert()
    {
        if (_itemCase == null)
            return null;
        
        return Convert(_itemCase);
    }

    public ItemCaseDto Convert(ItemCase itemCase)
    {
        ItemDto? itemDto = itemCase.Item != null ? 
            new ItemToDto(itemCase.Item).Convert() : null;
        CaseDto? caseDto = itemCase.Case != null ? 
            new CaseToDto(itemCase.Case).Convert() : null;

        return new ItemCaseDto()
        {
            ItemDto = itemDto,
            CaseDto = caseDto
        };
    }

    public List<ItemCaseDto> Convert(List<ItemCase> itemCases)
    {
        List<ItemCaseDto> result = [];
        foreach (ItemCase itemCase in itemCases) 
            result.Add(Convert(itemCase));

        return result;
    }
}