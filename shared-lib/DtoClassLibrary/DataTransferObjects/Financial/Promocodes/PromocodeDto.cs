using DtoClassLibrary.DataTransferObjects.Financial.Promocodes;

namespace DataTransferLib.DataTransferObjects.Financial.Promocodes;

public class PromocodeDto
{
    public string? Id { get; set; }
    public bool IsValid => TimesUsed < MaxTimesUsed && DateTime.UtcNow < ExpirationDate;
    public string Promocode { get; set; }
    public PromocodeType PromocodeType { get; set; }
    public int TimesUsed { get; set; }
    public int MaxTimesUsed { get; set; }
    public DateTime ExpirationDate { get; set; }
}