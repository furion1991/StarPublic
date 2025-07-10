using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersService.Models.DbModels.MinorBonuses;

[Table("prize_draws")]
public class PrizeDraw
{
    [Key]
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime DrawDate { get; set; }
    public decimal PrizeAmount { get; set; }
    public List<User> Participants { get; set; } = new List<User>();
    public bool IsFinished { get; set; } = false;
}