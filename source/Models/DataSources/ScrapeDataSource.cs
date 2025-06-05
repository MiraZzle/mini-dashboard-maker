using System;
using System.Data;
using System.Net.Http;
using System.Text.RegularExpressions;
using source.Models.DataSources;

namespace source.Models.DataSources;

public class ScrapeDataSource : IDataSource
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SourceName { get; set; } = string.Empty;
    public DataSourceType SourceType => DataSourceType.Scrape;

    public string TargetUrl { get; set; } = string.Empty;
    public string RegexPattern { get; set; } = string.Empty;
    public RegexOptions RegexOptions { get; set; } = RegexOptions.IgnoreCase;

    public async Task<DataTable> FetchDataAsync() {
        using var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(TargetUrl);

        var regex = new Regex(RegexPattern, RegexOptions);
        var matches = regex.Matches(html);

        var dataTable = new DataTable();

        // if no matches found, return empty DataTable
        if (matches.Count == 0)
            return dataTable;

        // use the first match to create columns
        var groupNames = regex.GetGroupNames().Where(n => !int.TryParse(n, out _)).ToList();

        foreach (var name in groupNames)
            dataTable.Columns.Add(name);

        foreach (Match match in matches) {
            var row = dataTable.NewRow();
            foreach (var name in groupNames) {
                row[name] = match.Groups[name].Value;
            }
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    public Dictionary<String, String> GetDescription() =>
        new Dictionary<String, String>() {
            { "URL", TargetUrl },
            { "Pattern", RegexPattern },
            { "Options", RegexOptions.ToString() }
        };
}
