using DataTransferLib.DataTransferObjects.Financial;
using DataTransferLib.DataTransferObjects.Financial.Models;
using FinancialService.Database.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;

namespace FinancialService.Converters;

/// <summary>Класс конвертера Transaction в TransactionDto</summary>
public class TransactionToDto(Transaction? transaction = null) : IConverter<Transaction, TransactionDto>
{
    private readonly Transaction? _transaction = transaction;

    public TransactionDto? Convert()
    {
        if (_transaction == null)
            return null;
        
        return Convert(_transaction);
    }

    public TransactionDto Convert(Transaction transaction)
    {
        FinancialDataDto? financialDataDto = transaction.FinancialData != null ? 
            new FinancialDataToDto(transaction.FinancialData).Convert() : null;

        return new TransactionDto()
        {
            Id = transaction.Id,
            FinancialData = financialDataDto,
            BalanceBefore = transaction.BalanceBefore,
            BalanceAfter = transaction.BalanceAfter,
            Amount = transaction.Amount,
            Type = transaction.Type,
            PaymentType = transaction.PaymentType,
            Timestamp = transaction.Timestamp
        };
    }

    public List<TransactionDto> Convert(List<Transaction> transactions)
    {
        List<TransactionDto> result = [];
        foreach (Transaction transaction in transactions) 
            result.Add(Convert(transaction));

        return result;
    }
}