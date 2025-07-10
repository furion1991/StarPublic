using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialService.Database.Models.Bonuses;

[Table("user_bonus_records")]
public class UserBonusRecord
{
    [Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("fin_data_id")] public string FinDataId { get; set; }
    [Column("bonus_id")] public string BonusId { get; set; }
    public Bonus Bonus { get; set; }
    [Column("is_wheel_bonus")] public bool IsWheelBonus { get; set; }
    [Column("time_got_bonus")] public DateTime TimeGotBonus { get; set; }
    public FinancialData FinancialData { get; set; }
    [Column("is_used")] public bool IsUsed { get; set; }
}

