namespace source.Services.Storage
{
    public interface IDataStorageService<T>
    {
        Task<List<T>> LoadAllAsync();
        Task SaveAllAsync(List<T> items);
    }
}
