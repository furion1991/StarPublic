using DataTransferLib.DataTransferObjects.Common;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DtoClassLibrary.DataTransferObjects.Common;

public static class Extensions
{

    public static async Task<IResponse<T>?> ReadResponse<T>(this HttpResponseMessage responseMessage)
    {
        var contentString = await responseMessage.Content.ReadAsStringAsync();

        try
        {
            var result = JsonConvert.DeserializeObject<ExtendedResponse<T>>(contentString);
            if (result is not null)
            {
                return result;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        try
        {
            var defaultResponse = JsonConvert.DeserializeObject<DefaultResponse<T>>(contentString);
            if (defaultResponse is not null)
            {
                return defaultResponse;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        try
        {
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse<T>>(contentString);
            if (errorResponse is not null)
            {
                return errorResponse;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return new ErrorResponse<T>
        {
            StatusCode = responseMessage.StatusCode,
            Result = default,
            Message = "Error!!!",
            ErrorDetails = contentString
        };
    }


    private static bool TryDeserialize<T>(string content, out T? result)
    {
        try
        {
            result = JsonSerializer.Deserialize<T>(content);
            return result is not null;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}