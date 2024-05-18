using Newtonsoft.Json;

namespace Soul.Shop.Infrastructure.Extensions;

public static class HttpClientExtensions
{
    private static readonly JsonSerializer s_jsonSerializer = new();

    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent httpContent)
    {
        await using var stream = await httpContent.ReadAsStreamAsync();
        var jsonReader = new JsonTextReader(new StreamReader(stream));

        return s_jsonSerializer.Deserialize<T>(jsonReader);
    }

    public static Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string url, T value)
    {
        return SendJsonAsync(client, HttpMethod.Post, url, value);
    }

    public static Task<HttpResponseMessage> PutJsonAsync<T>(this HttpClient client, string url, T value)
    {
        return SendJsonAsync(client, HttpMethod.Put, url, value);
    }

    private static Task<HttpResponseMessage> SendJsonAsync<T>(this HttpClient client, HttpMethod method, string url,
        T value)
    {
        var stream = new MemoryStream();
        var jsonWriter = new JsonTextWriter(new StreamWriter(stream));
        s_jsonSerializer.Serialize(jsonWriter, value);
        jsonWriter.Flush();
        stream.Position = 0;
        var request = new HttpRequestMessage(method, url)
        {
            Content = new StreamContent(stream)
        };

        request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");
        return client.SendAsync(request);
    }
}
