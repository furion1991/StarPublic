using System.Net;

namespace DataTransferLib.DataTransferObjects.Common;

/// <summary>Класс ответа для возврата результата из методов контроллеров. Нужен для создания корректного Json</summary>
public class DefaultResponse<T> : IResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public required string Message { get; set; }
    public T? Result { get; set; }
}

public interface IResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; }
    public T? Result { get; set; }
}