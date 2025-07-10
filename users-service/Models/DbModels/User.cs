using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using UsersService.Models.DbModels.MinorBonuses;

namespace UsersService.Models.DbModels
{
    [Table("users")]
    public class User
    {
        [Key, Required][Column("id")] public string? Id { get; set; }

        [Required]
        [EmailAddress]
        [Column("username")]
        public string? UserName { get; set; }

        [Column("email")] public string? Email { get; set; }

        [Column("image_path")] public string? ProfileImagePath { get; set; }

        [JsonIgnore][Column("phone")] public string? Phone { get; set; }

        public string BlockStatusId { get; set; }

        public BlockStatus? BlockStatus { get; set; }
        [Column("chance_boost")] public double ChanceBoost { get; set; } = 1;
        public string UserRoleId { get; set; }

        public UserRole? UserRole { get; set; }

        [Column("dateofregistration")] public DateTime DateOfRegistration { get; set; }

        [Column("isdeleted")] public bool IsDeleted { get; set; }

        public string UserInventoryId { get; set; }

        public UserInventory? UserInventory { get; set; }

        public string UserStatisticsId { get; set; }

        public DailyBonus DailyBonus { get; set; }
        public UserStatistics? UserStatistics { get; set; }
        public ICollection<ContractHistoryRecord> ContractHistoryRecords { get; set; } = new List<ContractHistoryRecord>();
        public ICollection<UpgradeHistoryRecord> UpgradeHistoryRecords { get; set; } = new List<UpgradeHistoryRecord>();
        public string? PriceDrawId { get; set; }
        public PrizeDraw? CurrentPriceDraw { get; set; }
        public ICollection<PrizeDrawResult> WonDraws { get; set; } = new List<PrizeDrawResult>();
    }
}