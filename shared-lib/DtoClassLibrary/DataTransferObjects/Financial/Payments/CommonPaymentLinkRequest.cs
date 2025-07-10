namespace DataTransferLib.DataTransferObjects.Financial.Payments;

public class CommonPaymentLinkRequest
{
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentLinkProvider { get; set; }
    public string PaymentType { get; set; }
    public string Email { get; set; }
}