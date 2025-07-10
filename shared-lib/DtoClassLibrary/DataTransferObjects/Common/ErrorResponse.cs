using System.Net;
using DataTransferLib.DataTransferObjects.CasesItems;

namespace DataTransferLib.DataTransferObjects.Common;

public class ErrorResponse<T> : IResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Result { get; set; }
    public string? ErrorDetails { get; set; }
}