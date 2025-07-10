namespace CasesService.Database.Models;

public class Multipliers
{
    public string Id { get; set; } = "global";
    public double NewPlayerMultiplier { get; set; }
    public int NewPlayerCasesRollCount { get; set; }
    public int NewPlayerUpgradesCount { get; set; }
    public int NewPlayerContractsCount { get; set; }
}