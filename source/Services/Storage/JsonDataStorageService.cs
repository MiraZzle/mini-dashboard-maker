using System.Text.Json;
using DataSourcess;
using Models.DataSources;
using source.Services.Storage;

namespace MiniDashboardMaker.Services.Storage;

public class JsonDataStorageService<T> : IDataStorageService<T>
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    public JsonDataStorageService(string filePath) {
        _filePath = filePath;
        _options = new JsonSerializerOptions {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
    }

    public async Task<List<T>> LoadAllAsync() {
        if (!File.Exists(_filePath))
            return new List<T>();

        var json = await File.ReadAllTextAsync(_filePath);

        if (typeof(T) == typeof(IDataSource)) {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var sources = DataSourceFactory.CreateMany(root);
            return sources.Cast<T>().ToList();
        }

        return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
    }

    public async Task SaveAllAsync(List<T> items) {
        var json = JsonSerializer.Serialize(items, _options);
        await File.WriteAllTextAsync(_filePath, json);
    }
}
