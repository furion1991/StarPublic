namespace DataTransferLib.DataTransferObjects.Audit;

public class ItemLogDto : BaseLogDto
{
    //> Енумерации

    /// <summary>Тип сообщения</summary>
    public enum ITYPE : int 
    {
        /// <summary>Создание предмета</summary>
        Create = 0,
        /// <summary>Редактирование предмета</summary>
        Change = 1,
        /// <summary>Изменение редкости предмета</summary>
        ChangeRarity = 2,
        /// <summary>Изменение шанса выпадения предмета</summary>
        ChangeChance = 3

    }
    //< Енумерации

    public required string ItemId { get; set; }
    
    public required ITYPE ItemLogType { get; set; }
}