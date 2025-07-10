using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DtoClassLibrary.DataTransferObjects.CasesItems;


/// <summary>Тип кейса</summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ECaseType : int
{
    None = 0,
    /// <summary>Обычный</summary>
    FirstCategory = 1,

    /// <summary>Премиальный</summary>
    SecondCategory = 2
}
