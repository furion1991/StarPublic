using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Financial.Models;

namespace DataTransferLib.DataTransferObjects.Financial;


/// <summary>Класс для получения корректных параметров запроса</summary>
public class BonusDefaultRequest : DefaultRequest
{
    public BTYPE BonusType { get; set; }
    public string FinancialDataId { get; set; }
}