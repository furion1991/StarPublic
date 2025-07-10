namespace DtoClassLibrary.DataTransferObjects.Audit.Admin.Fin;

public class BalanceDiversificationRecord
{
    public float Percentage { get; set; }
    public decimal Amount { get; set; }
    public BalanceType BalanceType { get; set; }
}

