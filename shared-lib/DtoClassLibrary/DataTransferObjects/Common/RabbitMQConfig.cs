using RabbitMQ.Client;

namespace DtoClassLibrary.DataTransferObjects.Common;

///<summary>Класс с задекларированными параметрами ecxhange и queue</summary>
public class RabbitMqConfig
{
    public const string LogsExchangeName = "logs";
    public const string CacheQueueName = "cache_queue";
    public const string CacheExchangeName = "cache_exchange";
    public const string ExchangeType = RabbitMQ.Client.ExchangeType.Direct;

    // Email Exchange (общий для всех email-сообщений)
    public const string EmailExchangeName = "email";

    // Очереди для email
    public const string EmailConfirmationQueueName = "email_confirmation";
    public const string PasswordResetQueueName = "password_reset";

    // Routing Keys
    public const string EmailConfirmationRoutingKey = "email.confirmation";
    public const string PasswordResetRoutingKey = "email.password_reset";

    // Настройки очередей
    public const bool Durable = true;
    public const bool AutoDelete = false;
    public const bool Exclusive = false;

    public const bool Persistent = true;
}