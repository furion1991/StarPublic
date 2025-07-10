using System.ComponentModel.DataAnnotations;

namespace AuthService.Database.Models;

public class ActiveUser
{
    [Key, Required]
    public string UserId { get; set; }
    public DateTime LoginTime { get; set; }
}
