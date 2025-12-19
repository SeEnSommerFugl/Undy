using System.Collections.ObjectModel;
using Undy.Data.Repository;

namespace UndyTest.TestDoubles;

internal sealed class ThrowingBaseRepository<T, TKey> : IBaseRepository<T, TKey>
    where T : class
    where TKey : notnull
{
    private readonly Exception _ex;

    public ThrowingBaseRepository(Exception ex)
    {
        _ex = ex;
        Items = new ObservableCollection<T>();
    }

    public ObservableCollection<T> Items { get; }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task<T?> GetByIdAsync(TKey id) => Task.FromResult<T?>(null);

    public Task<List<T>> GetByIdsAsync(IEnumerable<TKey> ids) => Task.FromResult(new List<T>());

    public Task AddAsync(T entity) => Task.FromException(_ex);

    public Task AddRangeAsync(IEnumerable<T> entities) => Task.FromException(_ex);

    public Task UpdateAsync(T entity) => Task.FromException(_ex);

    public Task UpdateRangeAsync(IEnumerable<T> entities) => Task.FromException(_ex);

    public Task DeleteAsync(TKey id) => Task.FromException(_ex);
}
