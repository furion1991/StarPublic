namespace DataTransferLib.DataTransferObjects.Audit;

public class CaseLogDto : BaseLogDto
{
    /// <summary>Тип сообщения</summary>
    public enum CTYPE : int
    {
        /// <summary>Создание кейса</summary>
        Create = 0,

        /// <summary>Изменение видимости кейса</summary>
        ChangeVisibility = 1,

        /// <summary>Изменение предметов кейса</summary>
        ChangeItems = 2,

        /// <summary>Изменение кейса</summary>
        ChangeCase = 3
    }
    //< Енумерации

    public required string CaseId { get; set; }

    public required CTYPE CaseLogType { get; set; }
}