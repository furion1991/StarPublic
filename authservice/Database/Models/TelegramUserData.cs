using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Database.Models;
public class TelegramUserData
{
    [Column("id")] public long Id { get; set; }
    [Column("first_name")] public string? FirstName { get; set; }
    [Column("last_name")] public string? LastName { get; set; }
    [Column("username")] public string? Username { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }

    public string MainUserId { get; set; }
    public StarDropUser StarDropUser { get; set; }
}

