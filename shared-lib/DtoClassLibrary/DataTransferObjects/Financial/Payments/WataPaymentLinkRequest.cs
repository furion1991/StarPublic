using System.Text.Json.Serialization;

namespace DataTransferLib.DataTransferObjects.Financial.Payments;

public class WataPaymentLinkRequest
{
    [JsonPropertyName("amount")] public double Amount { get; set; }
    [JsonPropertyName("currency")] public string Currency { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("orderId")] public string OrderId { get; set; }

    [JsonPropertyName("successRedirectUrl")]
    public string SuccessRedirectUrl { get; set; }

    [JsonPropertyName("failRedirectUrl")] public string FailRedirectUrl { get; set; }

    [JsonPropertyName("expirationDateTime")]
    public DateTime ExpirationDateTime { get; set; }
}