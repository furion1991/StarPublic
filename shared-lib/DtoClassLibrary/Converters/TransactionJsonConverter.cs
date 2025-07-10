using DataTransferLib.DataTransferObjects.Financial.Models;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DtoClassLibrary.Converters;
public class TransactionJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value, value?.GetType());
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {

        var jsonObject = JObject.Load(reader);

        if (jsonObject.TryGetValue("Type", out var typeToken))
        {
            var type = (TTYPE)typeToken.ToObject<int>();

            var target = type switch
            {
                TTYPE.Purchase when jsonObject.ContainsKey("CaseId") => new CasePurchaseTransactionParams(),
                _ => new TransactionParams()
            };

            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }

        throw new JsonSerializationException("Unable to determine transaction type");
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(TransactionParams).IsAssignableFrom(objectType);
    }
}

