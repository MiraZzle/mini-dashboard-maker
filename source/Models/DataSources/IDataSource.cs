using System.Data;
using source.Models.Shared;

namespace source.Models.DataSources;

public interface IDataSource : IDescribable
{
    Guid Id { get; set; }
    string SourceName { get; set; }

    DataSourceType SourceType { get; }

    Task<DataTable> FetchDataAsync();



}
