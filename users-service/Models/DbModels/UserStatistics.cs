using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace UsersService.Models.DbModels
{
    [Table("userstatistics")]
    public class UserStatistics
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("casesbought")]
        public int CasesBought { get; set; }

        [Column("ordersplaced")]
        public int OrdersPlaced { get; set; }

        [Column("crashrocketsplayed")]
        public int CrashRocketsPlayed { get; set; }

        [Column("luckbaraban")]
        public int LuckBaraban { get; set; }

        [Column("promocodesused")]
        public int PromocodesUsed { get; set; }

        [Column("userid")]
        public string? UserId { get; set; }

        [Column("fail_score")]
        public int FailScore { get; set; } = 0;

        [Column("total_cases_spent")]
        public decimal TotalCasesSpent { get; set; } = 0;

        [Column("total_cases_profit")]
        public decimal TotalCasesProfit { get; set; } = 0;

        [Column("contracts_placed")]
        public int ContractsPlaced { get; set; } = 0;

        [Column("total_contracts_spent")]
        public decimal TotalContractsSpent { get; set; } = 0;

        [Column("total_contracts_profit")]
        public decimal TotalContractsProfit { get; set; } = 0;

        [Column("upgrades_played")]
        public int UpgradesPlayed { get; set; } = 0;

        [JsonIgnore]
        public User? User { get; set; }
    }

}
