using System.Data;

namespace Models.DataSources;

public interface IDataSource
{
    Guid Id { get; set; }
    string SourceName { get; set; }

    DataSourceType SourceType { get; }

    Task<DataTable> FetchDataAsync();

}
