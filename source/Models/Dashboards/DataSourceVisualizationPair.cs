using source.Models.DataSources;

namespace source.Models.Dashboards
{
    public class DataSourceVisualizationPair
    {
        public Guid DataSourceId { get; set; }
        public VisualizationType Visualization { get; set; }
    }
}
