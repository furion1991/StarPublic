namespace DataTransferLib.DataTransferObjects.Financial.Models;

//> Енумерации

/// <summary>Тип транзакции</summary>
public enum TTYPE : int
{
    /// <summary>Депозит</summary>
    Deposit = 0,

    /// <summary>Покупка</summary>
    Purchase = 1,

    /// <summary>Вывод</summary>
    Withdraw = 2,
    /// <summary>
    /// Кэшбэк
    /// </summary>
    Cashback = 3,
    Bonus = 4,
    ItemSell = 5
}

/// <summary>Тип оплаты</summary>
public enum PTYPE : int
{
    /// <summary>Банковская карта</summary>
    Bank = 0,

    /// <summary>Криптовалюта</summary>
    CryptoCurrency = 1
}
//< Енумерации