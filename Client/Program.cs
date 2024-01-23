using System.Net;

int port = 3000;
HttpClient client = new()
{
    BaseAddress = new Uri($"http://127.0.0.1:{port}/")
};

await GetAsync(client);

static async Task GetAsync(HttpClient client)
{
    using HttpResponseMessage response = await client.GetAsync("users");
    
    response.EnsureSuccessStatusCode().WriteRequestToConsole();
    
    var jsonResponse = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"{jsonResponse}\n");
    
}

static class HttpResponseMessageExtensions
{
    internal static void WriteRequestToConsole(this HttpResponseMessage response)
    {
        if (response is null)
        {
            return;
        }

        var request = response.RequestMessage;
        Console.WriteLine($"Method: {request?.Method} ");
        Console.WriteLine($"Uri: {request?.RequestUri} ");
        Console.WriteLine($"version: HTTP/{request?.Version}");        
    }
}