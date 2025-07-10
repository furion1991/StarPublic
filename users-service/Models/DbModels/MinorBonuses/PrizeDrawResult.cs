using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersService.Models.DbModels.MinorBonuses;


[Table("prize_draw_results")]
public class PrizeDrawResult
{
    [Key]
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Winner { get; set; }
    public decimal PrizeAmount { get; set; }
    [ForeignKey("Winner")]
    public User? WinnerUser { get; set; }

    public DateTime DateDrawFinished { get; set; }
}