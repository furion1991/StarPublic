using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DataTransferLib.DataTransferObjects.Financial;
using FinancialService.Database.Models;

namespace FinancialService.Converters;

/// <summary>Класс конвертера FinancialData в FinancialDataDto</summary>
public class FinancialDataToDto(FinancialData? financialData = null) : IConverter<FinancialData, FinancialDataDto>
{
    private readonly FinancialData? _financialData = financialData;

    public FinancialDataDto? Convert()
    {
        if (_financialData == null)
            return null;
            
        return Convert(_financialData);
    }

    public FinancialDataDto Convert(FinancialData financialData)
    {
        return new FinancialDataDto()
        {
            Id = financialData.Id,
            UserId = financialData.UserId,
            CurrentBalance = financialData.CurrentBalance
        };
    }

    public List<FinancialDataDto> Convert(List<FinancialData> financialDatas)
    {
        List<FinancialDataDto> result = [];
        foreach (FinancialData financialData in financialDatas) 
            result.Add(Convert(financialData));

        return result;
    }
}