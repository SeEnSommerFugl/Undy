namespace Undy.Data.Repository
{
    public interface IBaseRepository<T, TKey> where T : class where TKey : notnull
    {
        Task InitializeAsync();
        ObservableCollection<T> Items { get; }
        Task<T?> GetByIdAsync(TKey id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(TKey id);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);
    }
}
