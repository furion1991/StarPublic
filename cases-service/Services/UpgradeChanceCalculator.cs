namespace CasesService.Services;

public static class UpgradeChanceCalculator
{
    public static decimal CalculateUpgradeChance(decimal userPrice, decimal attemptedPrice)
    {
        if (userPrice <= 0 || attemptedPrice <= 0)
        {
            return 0;
        }

        decimal baseChance = userPrice / attemptedPrice;
        decimal modifier = 0.9m;
        decimal chance = baseChance * modifier;

        return Math.Clamp(chance, 0.05m, 1.0m);
    }

    public static decimal CalculateCoefficient(decimal userPrice, decimal attemptedPrice)
    {
        if (userPrice <= 0)
        {
            return 0;
        }

        return Math.Round(attemptedPrice / userPrice, 2);
    }

    public static decimal CalculateChance(decimal userPrice, decimal attemptedPrice, decimal failScore, double boost)
    {
        decimal baseChance = userPrice / attemptedPrice;
        decimal modifier = 0.9m;
        decimal chance = baseChance * modifier;

        chance *= 1 + 0.3m * failScore;
        var decBoost = Convert.ToDecimal(boost);
        chance *= decBoost;

        return Math.Clamp(chance, 0.05m, 1.0m);
    }
}