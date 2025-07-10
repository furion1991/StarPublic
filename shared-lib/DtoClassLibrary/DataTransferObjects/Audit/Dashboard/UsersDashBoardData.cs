using DtoClassLibrary.DataTransferObjects.Users;

namespace DtoClassLibrary.DataTransferObjects.Audit.Dashboard;

public class UsersDashBoardData
{
    public int UsersCount { get; set; }
    public List<UserShortDto> UserData { get; set; } = [];
    public int NewUsersCount { get; set; }
    public List<UserShortDto> NewUsersData { get; set; } = [];

}

