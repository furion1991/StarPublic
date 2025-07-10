namespace DataTransferLib.DataTransferObjects.Audit;

public class BaseLogDto
{
    public string Id { get; set; }

    public string? Message { get; set; }

    public string? PerformedById { get; set; }

    public required DateTime DateOfLog { get; set; }
}