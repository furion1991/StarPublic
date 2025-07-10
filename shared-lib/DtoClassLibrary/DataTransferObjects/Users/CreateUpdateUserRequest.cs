namespace DataTransferLib.DataTransferObjects.Users
{
    //Модель данных для бд пользователей (UserBase)
    public class CreateUpdateUserRequest
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string ProfileImageUrl { get; set; } = "";
    }
}