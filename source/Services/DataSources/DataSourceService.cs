using MiniDashboardMaker.Services.Storage;
using source.Models.DataSources;
using source.Services.Storage;

namespace source.Services.DataSources;

public class DataSourceService
{
    private readonly JsonDataStorageService<IDataSource> _storage;

    public DataSourceService() {
        _storage = new JsonDataStorageService<IDataSource>("dataSources.json");
    }

    public Task<List<IDataSource>> GetAllAsync() => _storage.LoadAllAsync();

    public async Task AddAsync(IDataSource source) {
        var all = await _storage.LoadAllAsync();
        all.Add(source);
        await _storage.SaveAllAsync(all);
    }
}
