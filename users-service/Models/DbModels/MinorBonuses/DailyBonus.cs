using System.ComponentModel.DataAnnotations.Schema;

namespace UsersService.Models.DbModels.MinorBonuses;

[Table("daily_bonuses")]
public class DailyBonus
{
    [Column("id")] public required string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("streak")] public int Streak { get; set; }
    [Column("last_got_bonus")] public DateTime TimeGotLastBonus { get; set; }
    [Column("user_id")] public required string UserId { get; set; } = string.Empty;
    public User User { get; set; }


    public bool IsUsedToday => DateTime.UtcNow - TimeGotLastBonus < TimeSpan.FromHours(24);
}