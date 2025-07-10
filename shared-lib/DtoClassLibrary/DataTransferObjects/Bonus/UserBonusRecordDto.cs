
namespace DtoClassLibrary.DataTransferObjects.Bonus;
public class UserBonusRecordDto
{
    public string Id { get; set; }
    public string FinDataId { get; set; }
    public string BonusId { get; set; }
    public IBonusDto Bonus { get; set; }
    public DateTime TimeGotBonus { get; set; }

}

