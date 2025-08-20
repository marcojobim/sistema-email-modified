using var client = new HttpClient();

if (args.Length == 0)
{
    Console.WriteLine("Erro: URL não fornecida");
    return 1;
}
var url = args[0];

try
{
    client.Timeout = TimeSpan.FromSeconds(3);

    var response = await client.GetAsync(url);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Health check para {url} bem-sucedido. Status: {response.StatusCode}");
        return 0;
    }
    else
    {
        Console.WriteLine($"Health check para {url} falhou. Status: {response.StatusCode}");
        return 1;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Health check para {url} falhou com uma exceção: {ex.Message}");
    return 1;
}