using System.Text.Json;
using DataSourcess;
using source.Models.DataSources;
using source.Services.Storage;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


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
            Converters = { new JsonStringEnumConverter() },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver {
                Modifiers = { PolymorphicModifier }
            }
        };
    }

    private static void PolymorphicModifier(JsonTypeInfo typeInfo) {
        if (typeInfo.Type == typeof(IDataSource)) {
            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                DerivedTypes = {
                new JsonDerivedType(typeof(ApiDataSource), "Api"),
                new JsonDerivedType(typeof(DownloadDataSource), "Download"),
                new JsonDerivedType(typeof(ScrapeDataSource), "Scrape")
            }
            };
        }
    }


    public async Task<List<T>> LoadAllAsync() {
        if (!File.Exists(_filePath))
            return new List<T>();

        var json = await File.ReadAllTextAsync(_filePath);

        if (typeof(T) == typeof(IDataSource)) {
            var result = JsonSerializer.Deserialize<List<IDataSource>>(json, _options);
            if (result is null)
                return new List<T>();
            return result.Cast<T>().ToList();
        }


        return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
    }


    public async Task SaveAllAsync(List<T> items) {
        if (typeof(T) == typeof(IDataSource)) {
            var jsonSerialized = JsonSerializer.Serialize(items.Cast<IDataSource>().ToList(), _options);
            await File.WriteAllTextAsync(_filePath, jsonSerialized);
            return;
        }

        var json = JsonSerializer.Serialize(items, _options);
        await File.WriteAllTextAsync(_filePath, json);

    }
}
