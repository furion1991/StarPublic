using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace DataTransferLib.DataTransferObjects.CasesItems.Models;

public class ItemCaseDto : IDefaultDto
{
    public CaseDto? CaseDto { get; set; }
    public ItemDto? ItemDto { get; set; }
}