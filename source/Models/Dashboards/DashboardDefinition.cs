namespace source.Models.Dashboards
{
    public class DashboardDefinition
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public static int MaxPairs = 4;
        public List<DataSourceVisualizationPair> DataPairs { get; set; } = [];
    }
}
