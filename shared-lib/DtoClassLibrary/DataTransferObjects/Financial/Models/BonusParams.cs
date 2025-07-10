using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataTransferLib.DataTransferObjects.Financial.Models;

/// <summary>Класс параметров бонуса</summary>
public class BonusParams
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required BTYPE BonusType { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Percentage { get; set; }
    public TimeSpan? Duration { get; set; }
    public decimal? MinimumDeposit { get; set; }
    public decimal? BonusMultiplier { get; set; }
    public int? Count { get; set; }
    public int? ExtraSpins { get; set; }
}

/// <summary>Тип бонуса</summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum BTYPE : int
{
    /// <summary>Базовый тип</summary>
    Base = 0,
    /// <summary>Балансовый бонус</summary>
    Balance = 1,
    /// <summary>Бонус кэшбека</summary>
    Cashback = 2,
    /// <summary>Бонус к пополнению</summary>
    Deposit = 3,
    /// <summary>Бонус скидки на покупку</summary>
    Discount = 4,
    /// <summary>Бонус бесплатного кейса при пополнении</summary>
    FreeCase = 5,
    /// <summary>Бонус предметов при пополнении</summary>
    Item = 6,
    /// <summary>Бонус на случайный кейс</summary>
    Random = 7,
    /// <summary>Бонус для барабана</summary>
    WheelSpin = 8
}
//< Енумерации