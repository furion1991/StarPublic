using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DtoClassLibrary.DataTransferObjects.Auth.Telegram;

namespace ApiGateway.Services;
public static class TelegramHashValidator
{
    public static bool ValidateTelegramAuth(TelegramUserDataDto user, string botToken)
    {
        // 1️⃣ Создаем словарь с полями из DTO, кроме `hash`
        var dataDict = new Dictionary<string, string>
        {
            { "auth_date", user.AuthDate.ToString() },
            { "id", user.Id.ToString() }
        };

        if (!string.IsNullOrEmpty(user.FirstName))
            dataDict["first_name"] = user.FirstName;
        if (!string.IsNullOrEmpty(user.LastName))
            dataDict["last_name"] = user.LastName;
        if (!string.IsNullOrEmpty(user.Username))
            dataDict["username"] = user.Username;
        if (!string.IsNullOrEmpty(user.PhotoUrl))
            dataDict["photo_url"] = user.PhotoUrl;

        // 2️⃣ Сортируем ключи в алфавитном порядке и формируем строку
        var dataCheckString = string.Join("\n", dataDict.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
        Console.WriteLine($"Data Check String: {dataCheckString}");

        // 3️⃣ Генерируем secret_key = SHA256(bot_token)
        using var sha256 = SHA256.Create();
        byte[] secretKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(botToken));

        // 4️⃣ Генерируем HMAC-SHA256 hash
        using var hmac = new HMACSHA256(secretKey);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));

        // 5️⃣ Конвертируем в HEX
        var computedHashHex = BitConverter.ToString(computedHash).Replace("-", "").ToLower();
        Console.WriteLine($"Computed Hash: {computedHashHex}");
        Console.WriteLine($"Received Hash: {user.Hash}");

        // 6️⃣ Сравниваем с переданным `hash`
        return computedHashHex == user.Hash;
    }
}
