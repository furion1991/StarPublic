using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DataTransferLib.DataTransferObjects.Financial.Models;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using FinancialService.Database;
using FinancialService.Database.Models;
using FinancialService.Services;
using Microsoft.EntityFrameworkCore;
using TTYPE = DataTransferLib.DataTransferObjects.Financial.Models.TTYPE;

namespace FinancialService.Repositories;

public class TransactionRepository(ApplicationDbContext context, ILogger<TransactionRepository> logger, PaymentService paymentService) : IDisposable, IRepository<Transaction>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<Transaction> _entities = context.Transactions;
    private readonly ILogger<TransactionRepository> _logger = logger;
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task RevertLastTransaction(string userId)
    {
        var lastTransaction = await _entities
            .Include(e => e.FinancialData)
            .Where(t => t.FinancialData.UserId == userId)
            .OrderByDescending(t => t.Timestamp)
            .FirstOrDefaultAsync();

        var finData = lastTransaction.FinancialData;

        if (finData != null)
        {
            finData.CurrentBalance -= lastTransaction.Amount;
            _entities.Remove(lastTransaction);
        }


        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }

    public async Task<Transaction?> AddDeposit(TransactionParams? transactionParams,
        FinancialDataRepository financialRepo, bool flush = false)
    {
        if (transactionParams == null
            || transactionParams.Amount <= 0)
        {
            return null;
        }

        transactionParams.Type ??= TTYPE.Deposit;
        if (!string.IsNullOrEmpty(transactionParams.OrderId))
        {
            await paymentService.AcceptPayment(transactionParams.OrderId);
        }
        Transaction? transaction = await Add(transactionParams, financialRepo, flush);
        return transaction;
    }

    public async Task<Transaction?> AddCashback(TransactionParams transactionParams,
        FinancialDataRepository financialDataRepository, bool flush = false)
    {
        if (transactionParams == null
            || transactionParams.Amount <= 0)
            return null;

        transactionParams.Type = TTYPE.Cashback;
        Transaction? transaction = await Add(transactionParams, financialDataRepository, flush);
        return transaction;
    }

    public async Task<Transaction?> MakePurchase(TransactionParams? transactionParams,
        FinancialDataRepository financialRepo, bool flush = false)
    {
        if (transactionParams == null
        || transactionParams.Amount < 0)
            return null;

        transactionParams.Amount *= -1;
        transactionParams.Type = TTYPE.Purchase;
        Transaction? transaction = await Add(transactionParams, financialRepo, flush);
        return transaction;
    }

    public async Task<Transaction?> MakeWithdraw(TransactionParams? transactionParams,
        FinancialDataRepository financialRepo, bool flush = false)
    {
        if (transactionParams == null
        || transactionParams.Amount < 0)
            return null;

        transactionParams.Amount *= -1;
        transactionParams.Type = TTYPE.Withdraw;
        Transaction? transaction = await Add(transactionParams, financialRepo, flush);
        return transaction;
    }

    public async Task<Transaction?> Add(TransactionParams? transactionParams,
        FinancialDataRepository financialRepo, bool flush = false)
    {
        if (transactionParams == null
        || transactionParams.UserId == null
        || transactionParams.Amount == null
        || transactionParams.PaymentType == null)
            return null;

        FinancialData? financialData = await financialRepo.Get(transactionParams.UserId);
        if (financialData == null)
            return null;

        Transaction transaction = new()
        {
            FinancialData = financialData,
            Amount = (decimal)transactionParams.Amount,
            PaymentType = (PTYPE)transactionParams.PaymentType,
            BalanceBefore = financialData.CurrentBalance,
            BalanceAfter = financialData.CurrentBalance + (decimal)transactionParams.Amount,
            Type = (TTYPE)transactionParams.Type!
        };



        if (transactionParams.Type is TTYPE.Deposit or TTYPE.ItemSell)
        {
            financialData.CurrentBalance += (decimal)transactionParams.Amount;
        }
        else if (transaction.Type == TTYPE.Bonus)
        {
            financialData.BonusBalance += (decimal)transactionParams.Amount;
        }
        else if (transactionParams.Type is TTYPE.Purchase or TTYPE.Withdraw)
        {
            if (financialData.BonusBalance > 0 && financialData.BonusBalance - transactionParams.Amount >= 0)
            {
                financialData.BonusBalance += (decimal)transactionParams.Amount;
            }
            else
            {
                financialData.CurrentBalance += (decimal)transactionParams.Amount; // вот тут все равно выполняем сложение потому что выше сумма умножается на -1
            }
        }

        if (financialData.CurrentBalance < 0)
            return null;

        await Add(transaction);
        if (flush)
            await Save();

        await financialRepo.Update(financialData, flush);

        return transaction;
    }

    public async Task Add(Transaction transaction, bool flush = false)
    {
        await _entities.AddAsync(transaction);
        if (flush)
            await Save();
    }

    public async Task Remove(Transaction transaction, bool flush = false)
    {
        _entities.Remove(transaction);
        if (flush)
            await Save();
    }

    public async Task Update(Transaction transaction, bool flush = false)
    {
        _context.Entry(transaction).State = EntityState.Modified;
        if (flush)
            await Save();
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Transaction>> GetAll()
    {
        return await _entities.ToListAsync();
    }

    public async Task<Transaction?> Get(string id)
    {
        return await _entities.FindAsync(id);
    }

    public IQueryable<Transaction>? Filter(IQueryable<Transaction> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<Transaction> defaultWhere = objs.Where(transaction => transaction.FinancialDataId == defaultRequest.FilterValue);
        switch (defaultRequest.FilterBy.ToLower())
        {
            case "balance_before":
                decimal[]? balances = RequestService.GetValuesFromFilter<decimal>(defaultRequest.FilterValue);
                if (balances != null)
                    return objs.Where(transaction => transaction.BalanceBefore >= balances[0] && transaction.BalanceBefore <= balances[1]);
                else
                    return defaultWhere;
            case "balance_after":
                balances = RequestService.GetValuesFromFilter<decimal>(defaultRequest.FilterValue); ;
                if (balances != null)
                    return objs.Where(transaction => transaction.BalanceAfter >= balances[0] && transaction.BalanceAfter <= balances[1]);
                else
                    return defaultWhere;
            case "amount":
                decimal[]? amounts = RequestService.GetValuesFromFilter<decimal>(defaultRequest.FilterValue);
                if (amounts != null)
                    return objs.Where(transaction => transaction.Amount >= amounts[0] && transaction.BalanceAfter <= amounts[1]);
                else
                    return defaultWhere;
            case "type":
                if (Enum.TryParse(defaultRequest.FilterValue, out TTYPE type))
                    return objs.Where(transaction => transaction.Type == type);
                else
                    return defaultWhere;
            case "timestamp":
                DateTime[]? dates = RequestService.GetValuesFromFilter<DateTime>(defaultRequest.FilterValue);
                if (dates != null)
                    return objs.Where(transaction => transaction.Timestamp >= dates[0] && transaction.Timestamp <= dates[1]);
                else
                    return defaultWhere;
            default:
                return defaultWhere;
        }
    }

    public async Task<List<Transaction>> GetOrderBy(DefaultRequest defaultRequest)
    {
        IOrderedQueryable<Transaction> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("financial_data_id", "asc") => _entities.OrderBy(transaction => transaction.FinancialDataId),
            ("financial_data_id", "desc") => _entities.OrderByDescending(transaction => transaction.FinancialDataId),
            ("type", "asc") => _entities.OrderBy(transaction => transaction.Type),
            ("type", "desc") => _entities.OrderByDescending(transaction => transaction.Type),
            ("balance_before", "asc") => _entities.OrderBy(transaction => transaction.BalanceBefore),
            ("balance_before", "desc") => _entities.OrderByDescending(transaction => transaction.BalanceBefore),
            ("balance_after", "asc") => _entities.OrderBy(transaction => transaction.BalanceAfter),
            ("balance_after", "desc") => _entities.OrderByDescending(transaction => transaction.BalanceAfter),
            ("amount", "asc") => _entities.OrderBy(transaction => transaction.Amount),
            ("amount", "desc") => _entities.OrderByDescending(transaction => transaction.Amount),
            ("payment_type", "asc") => _entities.OrderBy(transaction => transaction.PaymentType),
            ("payment_type", "desc") => _entities.OrderByDescending(transaction => transaction.PaymentType),
            ("timestamp", "asc") => _entities.OrderBy(transaction => transaction.Timestamp),
            ("timestamp", "desc") => _entities.OrderByDescending(transaction => transaction.Timestamp),
            _ => _entities.OrderBy(transaction => transaction.Timestamp),
        };
        IQueryable<Transaction>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<List<Transaction>> GetOrderByWithFinancialData(FinancialData financialData, DefaultRequest defaultRequest)
    {
        IQueryable<Transaction> transaction = _entities.Where(transaction => transaction.FinancialData == financialData);//

        IOrderedQueryable<Transaction> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("financial_data_id", "asc") => transaction.OrderBy(transaction => transaction.FinancialDataId),
            ("financial_data_id", "desc") => transaction.OrderByDescending(transaction => transaction.FinancialDataId),
            ("type", "asc") => transaction.OrderBy(transaction => transaction.Type),
            ("type", "desc") => transaction.OrderByDescending(transaction => transaction.Type),
            ("balance_before", "asc") => transaction.OrderBy(transaction => transaction.BalanceBefore),
            ("balance_before", "desc") => transaction.OrderByDescending(transaction => transaction.BalanceBefore),
            ("balance_after", "asc") => transaction.OrderBy(transaction => transaction.BalanceAfter),
            ("balance_after", "desc") => transaction.OrderByDescending(transaction => transaction.BalanceAfter),
            ("amount", "asc") => transaction.OrderBy(transaction => transaction.Amount),
            ("amount", "desc") => transaction.OrderByDescending(transaction => transaction.Amount),
            ("payment_type", "asc") => transaction.OrderBy(transaction => transaction.PaymentType),
            ("payment_type", "desc") => transaction.OrderByDescending(transaction => transaction.PaymentType),
            ("timestamp", "asc") => transaction.OrderBy(transaction => transaction.Timestamp),
            ("timestamp", "desc") => transaction.OrderByDescending(transaction => transaction.Timestamp),
            _ => transaction.OrderBy(transaction => transaction.Timestamp),
        };
        IQueryable<Transaction>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<int> GetCountWithFinancialData(FinancialData financialData, DefaultRequest defaultRequest)
    {
        IQueryable<Transaction> transaction = _entities.Where(transaction => transaction.FinancialData == financialData);//

        IQueryable<Transaction>? where = Filter(transaction, defaultRequest);
        return await (where != null ? where.CountAsync() : transaction.CountAsync());
    }

    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<Transaction>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }
}