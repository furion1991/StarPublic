namespace DataTransferLib.DataTransferObjects.Audit;

public class UserLogDto : BaseLogDto
{
    //> Енумерации

    /// <summary>Тип сообщения</summary>
    public enum UTYPE : int
    {
        /// <summary>Создание пользователя</summary>
        Create = 0,

        /// <summary>Изменение данных пользователя</summary>
        Change = 1,

        /// <summary>Изменение роли блокировка/разблокировка</summary>
        ChangeBlocking = 2,

        /// <summary>Удаление пользователя</summary>
        Delete = 3,

        /// <summary>Изменение инвентаря пользователя</summary>
        ChangeInventory = 4
    }
    //< Енумерации

    public required string UserId { get; set; }

    public required UTYPE UserLogType { get; set; }
}