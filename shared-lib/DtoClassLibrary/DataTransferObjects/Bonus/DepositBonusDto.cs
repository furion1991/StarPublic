using System.ComponentModel.DataAnnotations.Schema;

namespace DtoClassLibrary.DataTransferObjects.Bonus
{
    public class DepositBonusDto : BonusBaseDto, IBonusDto
    {
        public decimal DepositCap { get; set; }
        public decimal BonusMultiplier { get; set; }
        public MultiplierType Mtype { get; set; }
    }
    public enum MultiplierType
    {
        Percentage,
        Multiply
    }
}
