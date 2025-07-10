using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DtoClassLibrary.DataTransferObjects.Bonus;

[JsonConverter(typeof(StringEnumConverter))]
public enum BonusType
{
    None,
    BalanceBonus,
    CashbackBonus,
    DepositBonus,
    DiscountBonus,
    FreeCaseBonus,
    ItemBonus,
    RandomCaseBonus,
    FreeSpinBonus,
    FiveKBonus,
    LetterBonus
}

