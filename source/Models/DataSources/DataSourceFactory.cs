using System.Text.Json;
using Models.DataSources;

namespace DataSourcess;

public static class DataSourceFactory
{
    public static IDataSource Create(DataSourceType type, JsonElement json) {
        return type switch {
            DataSourceType.Api => JsonSerializer.Deserialize<ApiDataSource>(json, JsonOptions())
                ?? throw new InvalidOperationException("Invalid ApiDataSource JSON"),

            DataSourceType.Download => JsonSerializer.Deserialize<DownloadDataSource>(json, JsonOptions())
                ?? throw new InvalidOperationException("Invalid DownloadDataSource JSON"),

            DataSourceType.Scrape => JsonSerializer.Deserialize<ScrapeDataSource>(json, JsonOptions())
                ?? throw new InvalidOperationException("Invalid ScrapeDataSource JSON"),

            _ => throw new NotSupportedException($"Unknown DataSourceType '{type}'")
        };
    }

    public static List<IDataSource> CreateMany(JsonElement jsonArray) {
        var result = new List<IDataSource>();

        foreach (var element in jsonArray.EnumerateArray()) {
            if (!element.TryGetProperty("sourceType", out var typeElement))
                throw new JsonException("Missing 'sourceType' in one of the elements.");

            var typeStr = typeElement.GetString();
            if (!Enum.TryParse<DataSourceType>(typeStr, ignoreCase: true, out var type))
                throw new JsonException($"Invalid sourceType: {typeStr}");

            var source = Create(type, element);
            result.Add(source);
        }

        return result;
    }

    private static JsonSerializerOptions JsonOptions() => new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };
}
