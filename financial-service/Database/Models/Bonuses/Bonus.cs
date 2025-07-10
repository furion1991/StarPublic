using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DtoClassLibrary.DataTransferObjects.Bonus;

namespace FinancialService.Database.Models.Bonuses;

/// <summary>Базовая сущность бонуса</summary>
[Table("bonus")]
public class Bonus
{
    [Key, Required]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("name")]
    public required string? Name { get; set; }
    [Column("description")]
    public string? Description { get; set; }
    [Column("bonus_image")] public string? BonusImage { get; set; }
    [Column("image_for_deposit_view")] public string? ImageForDepositView { get; set; }
    [Column("bonus_type")] public BonusType BonusType { get; set; }
    [Column("drop_chance")] public decimal DropChance { get; set; }
    [Column("is_deleted")] public bool IsDeleted { get; set; }








    public IBonusDto GetBonusDto()
    {
        return BonusType switch
        {
            BonusType.None => throw new ArgumentException("No default bonus entity allowed"),
            BonusType.BalanceBonus => ((BalanceBonus)this).ConvertToBonusDto(),
            BonusType.CashbackBonus => ((CashbackBonus)this).ConvertToBonusDto(),
            BonusType.DepositBonus => ((DepositBonus)this).ConvertToBonusDto(),
            BonusType.DiscountBonus => ((DiscountBonus)this).ConvertToBonusDto(),
            BonusType.FreeCaseBonus => ((FreeCaseBonus)this).ConvertToBonusDto(),
            BonusType.ItemBonus => ((ItemBonus)this).ConvertToBonusDto(),
            BonusType.RandomCaseBonus => ((RandomCaseBonus)this).ConvertToBonusDto(),
            BonusType.FreeSpinBonus => ((WheelSpinBonus)this).ConvertToBonusDto(),
            BonusType.LetterBonus => ((LetterBonus)this).ConvertToBonusDto(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}