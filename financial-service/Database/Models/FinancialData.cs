using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinancialService.Database.Models.Bonuses;
using Microsoft.EntityFrameworkCore;

namespace FinancialService.Database.Models;

/// <summary>Финансовые данные пользователя</summary>
[Table("financial_data")]
[Index(nameof(UserId), IsUnique = true)]
public class FinancialData
{
    [Key, Required][Column("id")] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column("user_id")] public required string UserId { get; set; }

    [Column("current_balance")] public decimal CurrentBalance { get; set; }

    [Column("bonus_balance")] public decimal BonusBalance { get; set; }

    [Column("transactions")] public ICollection<Transaction>? Transactions { get; set; }

    [Column("bonuses")] public ICollection<UserBonusRecord>? Bonuses { get; set; }
}