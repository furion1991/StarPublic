using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Database.Models;
public class VkUserData
{
    [Column("id")] public long Id { get; set; }
    [Column("photo_url")] public string? PhotoUrl { get; set; }
    [Column("first_name")] public string? FirstName { get; set; }
    [Column("main_user_id")] public string MainUserId { get; set; }
    public StarDropUser MainUser { get; set; }
    [Column("vk_refresh_token")] public string? VkRefReshToken { get; set; }
}

