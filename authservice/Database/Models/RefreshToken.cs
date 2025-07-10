using System.ComponentModel.DataAnnotations;

namespace AuthService.Database.Models;

public class RefreshToken
{
    [Key] public int Id { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public StarDropUser User { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
}