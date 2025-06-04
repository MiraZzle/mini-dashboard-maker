using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using source.Models.DataSources;

namespace source.Models.DataSources;

public class ApiDataSource : IDataSource
{
    public string ApiKey { get; set; } = string.Empty;
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SourceName { get; set; } = string.Empty;
    public DataSourceType SourceType => DataSourceType.Api;

    public string EndpointUrl { get; set; } = string.Empty;
    public HttpMethod Method { get; set; } = HttpMethod.Get;

    public Dictionary<string, string> Headers { get; set; } = new();
    public Dictionary<string, string> QueryParams { get; set; } = new();

    public string? Body { get; set; } = null;
    public string? JsonRootPath { get; set; } = null;

    public async Task<DataTable> FetchDataAsync() {
        using var httpClient = new HttpClient();

        var uriBuilder = new UriBuilder(EndpointUrl);
        if (QueryParams.Any()) {
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var kv in QueryParams) {
                query[kv.Key] = kv.Value;
            }
            uriBuilder.Query = query.ToString();
        }

        var request = new HttpRequestMessage(Method, uriBuilder.Uri);

        foreach (var header in Headers) {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (Method == HttpMethod.Post || Method == HttpMethod.Put || Method == HttpMethod.Patch) {
            if (!string.IsNullOrEmpty(Body)) {
                request.Content = new StringContent(Body, Encoding.UTF8, "application/json");
            }
        }

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return ParseJson(content);
    }

    private DataTable ParseJson(string jsonContent) {
        var dataTable = new DataTable();
        var jsonDoc = JsonDocument.Parse(jsonContent);
        JsonElement root = jsonDoc.RootElement;

        if (!string.IsNullOrWhiteSpace(JsonRootPath)) {
            var pathParts = JsonRootPath.Split('.');
            foreach (var part in pathParts) {
                if (root.TryGetProperty(part, out var next))
                    root = next;
                else
                    throw new InvalidOperationException($"Json path '{JsonRootPath}' not found.");
            }
        }

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

    public string GetDescription() =>
        $"API Source\n• Endpoint: {EndpointUrl}\n• API Key: {(string.IsNullOrEmpty(ApiKey) ? "None" : "Provided")}";
}
