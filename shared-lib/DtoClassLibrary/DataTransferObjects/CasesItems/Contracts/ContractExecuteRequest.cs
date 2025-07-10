namespace DtoClassLibrary.DataTransferObjects.CasesItems.Contracts;

public class ContractExecuteRequest
{
    public List<string> ItemRecordIds { get; set; } = new List<string>();
    public required string UserId { get; set; }
}