using System.ComponentModel.DataAnnotations.Schema;

namespace UsersService.Models.DbModels;


[Table("contracts_records")]
public class ContractHistoryRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public List<string> ItemsFromIds { get; set; }
    public string ResultItemId { get; set; }
    public DateTime DateOfContract { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}