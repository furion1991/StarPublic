using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using DataTransferLib.DataTransferObjects.Financial.Payments;
using FinancialService.Database.Models;
using Newtonsoft.Json;

namespace FinancialService.Services;

public class WataPaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WataPaymentService> _logger;
    public WataPaymentService(ILogger<WataPaymentService> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.wata.pro/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("WATA_API_KEY"));
    }
    public async Task<string> GetPaymentLink(PaymentOrder order, string userEmail)
    {
        var requestContent = new WataPaymentLinkRequest()
        {
            Currency = "RUB",
            Amount = Convert.ToDouble(order.Amount),
            Description = $"Оплата заказа для пользователя {userEmail}",
            OrderId = order.Id,
            FailRedirectUrl = $"https://front.dev.stardrop.app/payment/fail?orderId={order.Id}&userId={order.UserId}&amount={order.Amount}",
            SuccessRedirectUrl = $"https://front.dev.stardrop.app/payment/success?orderId={order.Id}&userId={order.UserId}&={order.Amount}",
            ExpirationDateTime = DateTime.UtcNow + TimeSpan.FromDays(15)

        };
        var content = new StringContent(JsonConvert.SerializeObject(requestContent), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/h2h/links", content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var paymentLinkResponse = JsonConvert.DeserializeObject<WataPaymentLinkResponse>(responseContent);
            if (paymentLinkResponse != null)
            {
                _logger.LogInformation("Payment link created successfully for user {UserId} with order {OrderId}", order.UserId, order.Id);
                order.ExpirationDate = paymentLinkResponse.ExpirationDateTime;
                return paymentLinkResponse.Url;
            }
            _logger.LogError("Failed to deserialize payment link response for user {UserId} with order {OrderId}", order.UserId, order.Id);
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();

            var error = JsonConvert.DeserializeObject<WataErrorResponse>(errorResponse);
            _logger.LogError("Failed to create payment link for user {UserId} with order {OrderId}. Status code: {StatusCode}, Reason: {ReasonPhrase}",
                order.UserId, order.Id, response.StatusCode, response.ReasonPhrase);

            _logger.LogError($"{JsonConvert.SerializeObject(error)}");
        }
        return "";
    }
}



public class WataErrorResponse
{
    [JsonPropertyName("error")]
    public WataErrorDetail Error { get; set; }
}

public class WataErrorDetail
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("details")]
    public string Details { get; set; }

    [JsonPropertyName("validationErrors")]
    public List<WataValidationError> ValidationErrors { get; set; }
}

public class WataValidationError
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("members")]
    public List<string> Members { get; set; }
}
