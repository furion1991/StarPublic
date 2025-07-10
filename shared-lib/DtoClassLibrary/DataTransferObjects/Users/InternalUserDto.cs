using DataTransferLib.DataTransferObjects.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace DtoClassLibrary.DataTransferObjects.Users;

public class InternalUserDto
{
    public UserDto PublicData { get; set; } = new();
    public int FailScore { get; set; }
    public decimal TotalCasesSpent { get; set; }
    public decimal TotalCasesProfit { get; set; }
    public double ChanceBoost { get; set; }
}