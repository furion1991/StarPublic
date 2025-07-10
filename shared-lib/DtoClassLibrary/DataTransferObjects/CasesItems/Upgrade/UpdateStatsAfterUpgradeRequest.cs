using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace DtoClassLibrary.DataTransferObjects.CasesItems.Upgrade;

public class UpdateStatsAfterUpgradeRequest
{
    public required string UserId { get; set; }

    /// <summary>
    /// Увеличить количество сыгранных контрактов
    /// </summary>
    public int AddContractsPlaced { get; set; } = 0;

    /// <summary>
    /// Увеличить значение fail score, если контракт неудачный
    /// </summary>
    public int AddFailScore { get; set; } = 0;

    /// <summary>
    /// Обнулить fail score (если контракт удачный)
    /// </summary>
    public bool ResetFailScore { get; set; } = false;

    /// <summary>
    /// Добавить в статистику суммарную потраченную стоимость предметов
    /// </summary>
    public decimal AddSpent { get; set; } = 0;

    /// <summary>
    /// Добавить в статистику профит (выигрыш - потраченное)
    /// </summary>
    public decimal AddProfit { get; set; } = 0;

    public required ItemDto ItemSpent { get; set; }
    public required ItemDto ItemGot { get; set; }

}