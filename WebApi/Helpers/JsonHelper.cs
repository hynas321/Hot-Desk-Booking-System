using System.Text.Json;

namespace WebApi.Helpers;

public class JsonHelper
{
    public static string Serialize(object o)
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Serialize(o, jsonSerializerOptions);
    }
}