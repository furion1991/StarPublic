using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Database.Models;

public class StarDropUser : IdentityUser
{
    [Column("tg_user_data_id")] public long TelegramUserDataId { get; set; }
    public TelegramUserData TelegramUserData { get; set; }
    [Column("vk_user_data_id")] public long VkUserDataId { get; set; }
    public VkUserData VkUserData { get; set; }
    [Column("last_time_password_changed")] public DateTime LastTimePasswordChanged { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [Column("vk_subscription_status")] public bool VkSubscribed { get; set; }
    [Column("tg_subscription_status")] public bool TgSubscribed { get; set; }
}