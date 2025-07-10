using DataTransferLib.DataTransferObjects.Common;

namespace DataTransferLib.DataTransferObjects.Audit;


/// <summary>Класс для получения корректных параметров запроса</summary>
public class LogDefaultRequest : DefaultRequest
{
    public LTYPE LogType { get; set; }
}