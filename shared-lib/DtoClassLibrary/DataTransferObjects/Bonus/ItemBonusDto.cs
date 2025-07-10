
using System.ComponentModel.DataAnnotations.Schema;

namespace DtoClassLibrary.DataTransferObjects.Bonus
{
    public class ItemBonusDto : BonusBaseDto, IBonusDto
    {
        public int ItemCount { get; set; }
        public decimal MinimumDeposit { get; set; }
        public decimal MinimalItemCost { get; set; }
        public decimal MaximalItemCost { get; set; }
        public bool IsDepositDependent { get; set; }

    }
}
