using System.Text.Json.Serialization;

namespace DataTransferLib.DataTransferObjects.Financial.Payments;

public class WataPaymentLinkResponse
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("amount")] public double Amount { get; set; }
    [JsonPropertyName("currency")] public string Currency { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; }
    [JsonPropertyName("url")] public string Url { get; set; }
    [JsonPropertyName("terminalName")] public string TerminalName { get; set; }
    [JsonPropertyName("terminalPublicId")] public Guid TerminalPublicId { get; set; }
    [JsonPropertyName("creationTime")] public DateTime CreationTime { get; set; }
    [JsonPropertyName("orderId")] public string OrderId { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; }
    [JsonPropertyName("successRedirectUrl")] public string SuccessRedirectUrl { get; set; }
    [JsonPropertyName("failRedirectUrl")] public string FailRedirectUrl { get; set; }
    [JsonPropertyName("expirationDateTime")] public DateTime ExpirationDateTime { get; set; }
}