using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace UsersService.Models.DbModels
{
    [Table("userrole")]
    public class UserRole
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("name")]
        public string? Name { get; set; }

        [Column("userid")]
        public string? UserId { get; set; }

        [JsonIgnore] public User? User { get; set; }
    }
}
