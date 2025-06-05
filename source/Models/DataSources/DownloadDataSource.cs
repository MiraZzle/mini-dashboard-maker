using System.Data;
using System.Net.Http;
using System.Text.Json;
using CsvHelper;
using System.Globalization;
using System.Text;
using source.Models.DataSources;

namespace source.Models.DataSources;

public class DownloadDataSource : IDataSource
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SourceName { get; set; } = string.Empty;
    public DataSourceType SourceType => DataSourceType.Download;

    public string FileUrl { get; set; } = string.Empty;
    public PermittedDownloadType Format { get; set; } = PermittedDownloadType.Csv;

    public async Task<DataTable> FetchDataAsync() {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(FileUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return Format switch {
            PermittedDownloadType.Csv => ParseCsv(content),
            PermittedDownloadType.Json => ParseJson(content),
            PermittedDownloadType.Xml => throw new NotImplementedException("XML support not implemented"),
            _ => throw new NotSupportedException($"Format '{Format}' is not supported.")
        };
    }

    private DataTable ParseCsv(string csvContent) {
        var dataTable = new DataTable();
        using var reader = new StringReader(csvContent);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        using var dr = new CsvDataReader(csv);
        dataTable.Load(dr);
        return dataTable;
    }

    private DataTable ParseJson(string jsonContent) {
        var dataTable = new DataTable();
        var jsonDoc = JsonDocument.Parse(jsonContent);
        var root = jsonDoc.RootElement;

        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0) {
            var firstRow = root[0];
            foreach (var prop in firstRow.EnumerateObject()) {
                dataTable.Columns.Add(prop.Name);
            }

            foreach (var item in root.EnumerateArray()) {
                var row = dataTable.NewRow();
                foreach (var prop in item.EnumerateObject()) {
                    row[prop.Name] = prop.Value.ToString();
                }
                dataTable.Rows.Add(row);
            }
        }

        return dataTable;
    }

    public Dictionary<String, String> GetDescription() =>
        new Dictionary<String, String> {
            { "File URL", FileUrl },
            { "Format", Format.ToString() }
        };
}
