namespace DtoClassLibrary.DataTransferObjects.Audit;

public class OpenedCasesRequest
{
    public string FilterBy { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}