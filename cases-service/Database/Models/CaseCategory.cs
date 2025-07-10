namespace CasesService.Database.Models;

public class CaseCategory
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public required string NormilizedName { get; set; }
    public ICollection<Case> Cases { get; set; } = new List<Case>();
    public string ImageUrl { get; set; }
}