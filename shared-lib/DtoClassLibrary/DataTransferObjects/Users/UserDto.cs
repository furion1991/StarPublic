using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace DtoClassLibrary.DataTransferObjects.Users;

public class UserDto
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? ProfileImagePath { get; set; }
    public string? Phone { get; set; }
    public decimal CurrentBalance { get; set; }
    public BlockStatusDto? BlockStatus { get; set; }
    public UserRoleDto? UserRole { get; set; }
    public DateTime DateOfRegistration { get; set; }
    public bool IsDeleted { get; set; }
    public InventoryRecordDto? UserInventory { get; set; }
    public UserStatisticsResponse? UserStatistics { get; set; }
    public List<ContractHistoryRecordDto> ContractHistoryRecords { get; set; } = new List<ContractHistoryRecordDto>();
    public List<UpgradeHistoryRecordDto> UpgradeHistoryRecords { get; set; } = new List<UpgradeHistoryRecordDto>();
    public DailyBonusDto? DailyBonus { get; set; } = new();
    public bool IsSubscribedToTg { get; set; }
    public bool IsSubscribedToVk { get; set; }
}

public class DailyBonusDto
{
    public string Id { get; set; }
    public DateTime Date { get; set; }
    public int Streak { get; set; }
    public decimal Amount { get; set; }
    public bool IsUsedToday { get; set; }
    public string? UserId { get; set; }
}

public class ContractHistoryRecordDto
{
    public ItemDto ResultItem { get; set; }
    public List<ItemDto> ItemsUsedOnThisContract { get; set; } = new List<ItemDto>();
    public DateTime DateOfContract { get; set; }
}

public class UpgradeHistoryRecordDto
{
    public ItemDto ResultItem { get; set; }
    public ItemDto ItemUsedOnThisUpgrade { get; set; }
    public DateTime DateOfUpgrade { get; set; }
    public bool IsSuccessful { get; set; }
    public decimal Chance { get; set; }
}

public class UserStatisticsResponse
{
    public string Id { get; set; }

    public int CasesBought { get; set; }

    public int OrdersPlaced { get; set; }

    public int CrashRocketsPlayed { get; set; }

    public int LuckBaraban { get; set; }

    public int PromocodesUsed { get; set; }

    public int ContractsPlaced { get; set; }

    public int UpgradesPlayed { get; set; }

    public decimal TotalCasesSpent { get; set; }

    public decimal TotalCasesProfit { get; set; }

    public decimal TotalContractsSpent { get; set; }

    public decimal TotalContractsProfit { get; set; }
}

public class InventoryRecordDto
{
    public string Id { get; set; }

    public ICollection<ItemRecordDto>? ItemsUserInventory { get; set; }
}

public class ItemRecordDto
{
    public string Id { get; set; }
    public string? Userinventoryid { get; set; }
    public ItemRecordState ItemRecordState { get; set; }
    public ItemDto ItemDto { get; set; }
    public string? ItemId { get; set; }
}

public class UserRoleDto
{
    public string Id { get; set; }

    public string? Name { get; set; }
}

public class BlockStatusDto
{
    public string Id { get; set; }

    public bool IsBlocked { get; set; }

    public string? Reason { get; set; }

    public string? PerformedById { get; set; }
}