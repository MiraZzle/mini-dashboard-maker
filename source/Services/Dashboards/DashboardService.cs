using MiniDashboardMaker.Services.Storage;
using source.Models.Dashboards;
using source.Services.Storage;

namespace source.Services.Dashboards;

public class DashboardService
{
    private readonly JsonDataStorageService<DashboardDefinition> _storage;

    public DashboardService() {
        _storage = new JsonDataStorageService<DashboardDefinition>("dashboards.json");
    }

    public Task<List<DashboardDefinition>> GetAllAsync() => _storage.LoadAllAsync();

    public async Task AddAsync(DashboardDefinition dashboard) {
        var all = await _storage.LoadAllAsync();
        all.Add(dashboard);
        await _storage.SaveAllAsync(all);
    }

    public Task<DashboardDefinition?> GetByIdAsync(Guid id) => _storage.GetByIdAsync(id);

    public Task RemoveAsync(Guid id) => _storage.RemoveByIdAsync(id);
}
