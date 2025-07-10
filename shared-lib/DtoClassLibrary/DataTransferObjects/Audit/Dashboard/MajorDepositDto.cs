using DtoClassLibrary.DataTransferObjects.Users;

namespace DtoClassLibrary.DataTransferObjects.Audit.Dashboard;

public class MajorDepositDto
{
    public decimal MajorDepositAmount { get; set; }
    public List<UserDto> Users { get; set; } = new List<UserDto>();
}

