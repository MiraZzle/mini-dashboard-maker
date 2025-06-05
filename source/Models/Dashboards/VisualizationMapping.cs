using source.Models.DataSources;

namespace source.Models.Dashboards
{
    public static class VisualizationMapping
    {
        private static readonly Dictionary<DataSourceType, List<VisualizationType>> _map = new() {
            [DataSourceType.Api] = new() { VisualizationType.LineChart, VisualizationType.BarChart },
            [DataSourceType.Download] = new() { VisualizationType.Table, VisualizationType.PieChart },
            [DataSourceType.Scrape] = new() { VisualizationType.CardList, VisualizationType.Summary }
        };

        public static List<VisualizationType> GetSupported(DataSourceType type) {
            return _map.TryGetValue(type, out var list) ? list : new();
        }
    }
}
