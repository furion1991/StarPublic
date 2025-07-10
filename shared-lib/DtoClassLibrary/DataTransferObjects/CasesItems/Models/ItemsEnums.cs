namespace DtoClassLibrary.DataTransferObjects.CasesItems.Models;

//> Енумерации

/// <summary>Тип предмета</summary>
public enum ItemType : int
{
    /// <summary>Ключ</summary>
    Key = 0,

    /// <summary>Вещь</summary>
    Stuff = 1,

    /// <summary>Кристалл</summary>
    Crystal = 2
}

/// <summary>Степень редкости предмета</summary>
public enum EItemRarity : int
{
    /// <summary>Обычный</summary>
    Standard = 0,
    /// <summary>Редкий</summary>
    Rare = 1,
    /// <summary>Эпический</summary>
    Epic = 2,
    /// <summary>Ультраредкий</summary>
    UltraRare = 3,
    /// <summary>Легендарные</summary>
    Legendary = 4,
    /// <summary>Мифические</summary>
    Mythical = 5,
}
//< Енумерации