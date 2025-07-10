using System.Net;

namespace DataTransferLib.DataTransferObjects.Common;

public class ActionResult
{
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
}