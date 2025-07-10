using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace UsersService.Models.DbModels
{
    [Table("blockstatus")]
    public class BlockStatus
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("isblocked")]
        public bool IsBlocked { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("performedbyid")]
        public string? PerformedById { get; set; }

        [Column("userid")]
        public string? UserId { get; set; }

        [JsonIgnore] public User? User { get; set; }
    }
}
