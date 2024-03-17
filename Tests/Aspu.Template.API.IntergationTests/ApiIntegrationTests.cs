using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Aspu.Template.API.IntergationTests;

public class ApiIntegrationTests : IDisposable
{
    private HttpClient _client;
    private IntegrationTestWebApplicationFactory _factory;

    private readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    [OneTimeSetUp]
    public void OneTimeSetup() => _factory = new IntegrationTestWebApplicationFactory();

    [SetUp]
    public void Setup() => _client = _factory.CreateClient();

    protected async Task<T?> GetRequest<T>(string path)
    {
        using var response = await _client.GetAsync(path);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T?>(content);
        return result;
    }

    protected async Task<T?> PostRequest<T>(string path, object body)
    {
        using StringContent json = new(JsonSerializer.Serialize(body, JsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);
        using var response = await _client.PostAsync(path, json);
        var content = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<T?>(content);
        return result;
    }

    private bool _disposedValue;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;

        if (disposing)
            _factory?.Dispose();

        _disposedValue = true;
    }
}