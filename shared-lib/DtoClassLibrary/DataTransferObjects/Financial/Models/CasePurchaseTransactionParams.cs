namespace DtoClassLibrary.DataTransferObjects.Financial.Models;
public class CasePurchaseTransactionParams : TransactionParams
{
    public string CaseId { get; set; }
    public int Quantity { get; set; }
}

