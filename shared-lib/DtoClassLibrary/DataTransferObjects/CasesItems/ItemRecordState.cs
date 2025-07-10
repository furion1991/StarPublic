namespace DtoClassLibrary.DataTransferObjects.CasesItems;

public enum ItemRecordState
{
    None = 0,
    FromCase = 1,
    FromContract = 2,
    FromUpgrade = 3,
    Withdrawn = 4,
    Sold = 5,
    UsedOnContract = 6,
    UsedOnUpgrade = 7,
}