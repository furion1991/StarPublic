using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DtoClassLibrary.DataTransferObjects.Bonus
{
    public class AllBonusDto
    {
        public BonusType BonusType { get; set; }
        public Dictionary<string, object> Params { get; set; }

        public IBonusDto DeserializeDto()
        {
            IBonusDto bonus = BonusType switch
            {
                BonusType.None => throw new ArgumentException("Not valid bonus type"),
                BonusType.BalanceBonus => new BalanceBonusDto()
                {
                    Name = Params["name"]?.ToString(),
                    BonusType = BonusType,
                    Description = Params["description"]?.ToString(),
                    Amount = Convert.ToDecimal(Params["amount"]),
                },
                BonusType.CashbackBonus => new CashbackBonusDto()
                {
                    Name = Params["name"]?.ToString(),
                    BonusType = BonusType,
                    Description = Params["description"]?.ToString(),
                    CashbackPercentage = Convert.ToDecimal(Params["cashback_percentage"]),
                    Duration = TimeSpan.Parse((string)Params["duration"]),
                },
                BonusType.DepositBonus => new DepositBonusDto()
                {
                    Name = Params["name"]?.ToString(),
                    BonusType = BonusType,
                    Description = Params["description"]?.ToString(),
                    BonusMultiplier = Convert.ToDecimal(Params["bonus_multiplier"]),
                    DepositCap = Convert.ToDecimal(Params["deposit_cap"]),
                    Mtype = Enum.Parse<MultiplierType>(Params["m_type"]?.ToString() ?? string.Empty)
                },
                BonusType.DiscountBonus => new DiscountBonusDto()
                {
                    Name = Params["name"]?.ToString(),
                    BonusType = BonusType,
                    Description = Params["description"]?.ToString(),
                    DiscountPercentage = Convert.ToDecimal(Params["discount_percentage"])
                },
                BonusType.FreeCaseBonus => new FreeCaseBonusDto()
                {
                    Name = Params["name"].ToString(),
                    BonusType = BonusType,
                    Description = Params["description"]?.ToString(),
                    MinimumDeposit = Convert.ToDecimal(Params["minimum_deposit"]),
                    CaseCount = Convert.ToInt32(Params["case_count"]),
                },
                BonusType.ItemBonus => new ItemBonusDto()
                {
                    Name = Params["name"].ToString(),
                    BonusType = BonusType,
                    Description = Params["description"]?.ToString(),
                    MinimumDeposit = Convert.ToDecimal(Params["minimum_deposit"]),
                    ItemCount = Convert.ToInt32(Params["item_count"])
                },
                BonusType.RandomCaseBonus => new RandomCaseBonusDto()
                {
                    Name = Params["name"].ToString(),
                    BonusType = BonusType,
                    Description = Params["description"]?.ToString(),
                    MinimumDeposit = Convert.ToDecimal(Params["minimum_deposit"]),
                },
                BonusType.FreeSpinBonus => new WheelSpinBonusDto()
                {
                    Name = Params["name"].ToString(),
                    BonusType = BonusType,
                    Description = Params["description"]?.ToString(),
                    ExtraSpins = Convert.ToInt32(Params["extra_spins"]),
                },
                _ => throw new ArgumentOutOfRangeException()
            };

            return bonus;
        }
    }
}
