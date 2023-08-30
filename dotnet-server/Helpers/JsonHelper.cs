using System.Text.Json;

class JsonHelper
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