using System.Net;

using DataTransferLib.DataTransferObjects.CasesItems;

namespace DataTransferLib.DataTransferObjects.Common;

public class ExtendedResponse<T> : IResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public required string Message { get; set; }
    public T? Result { get; set; }
    public int? Page { get; set; }
    public int? Count { get; set; }
}