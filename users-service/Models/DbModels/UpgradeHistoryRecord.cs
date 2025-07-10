namespace UsersService.Models.DbModels;

public class UpgradeHistoryRecord
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public required string ItemSpentId { get; set; }
    public required string ItemResultId { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime DateOfUpgrade { get; set; }
    public required string UserId { get; set; }
    public User User { get; set; }
}