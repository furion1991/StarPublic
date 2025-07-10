namespace DtoClassLibrary.DataTransferObjects.Common.Interfaces;
public interface IService
{
    Task SendLogMessage(string message, string queueName);
}