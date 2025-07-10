namespace DtoClassLibrary.DataTransferObjects.Users.Admin;

public class ManagerUserDto : SingleUserForAdminDto
{
    public List<WithdrawalDto> AssignedWithdrawals { get; set; } = [];
}