using CasesService.Database.Models;
using System.Security.Cryptography;

namespace CasesService.Utility;


public static class RandomStardropNumberGenerator
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
    public static double NextSecureDouble()
    {
        var bytes = new byte[8];
        _rng.GetBytes(bytes);
        ulong uint64 = BitConverter.ToUInt64(bytes, 0);
        return (double)uint64 / ulong.MaxValue;
    }
}


public class ItemDropper
{
    private readonly List<Item> _items;

    public ItemDropper(List<Item> items)
    {
        _items = items;
    }

    public Item? GetRandomItemByDropChance()
    {
        var items = _items.ToList();

        if (items.Count == 0)
        {
            // логнем
            Console.WriteLine("WARNING: Attempted to drop from an empty item list!");
            return null;
        }

        return items[new Random().Next(items.Count)];
    }

}