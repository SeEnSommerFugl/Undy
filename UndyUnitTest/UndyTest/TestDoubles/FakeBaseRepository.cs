using System.Collections.ObjectModel;
using Undy.Data.Repository;

namespace UndyTest.TestDoubles;

internal sealed class FakeBaseRepository<T, TKey> : IBaseRepository<T, TKey>
    where T : class
    where TKey : notnull
{
    private readonly Func<T, TKey> _getKey;

    public FakeBaseRepository(Func<T, TKey> getKey)
    {
        _getKey = getKey;
        Items = new ObservableCollection<T>();
    }

    public ObservableCollection<T> Items { get; }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task<T?> GetByIdAsync(TKey id)
        => Task.FromResult(Items.FirstOrDefault(x => EqualityComparer<TKey>.Default.Equals(_getKey(x), id)));

    public Task<List<T>> GetByIdsAsync(IEnumerable<TKey> ids)
    {
        var set = ids.ToHashSet();
        return Task.FromResult(Items.Where(x => set.Contains(_getKey(x))).ToList());
    }

    public Task AddAsync(T entity)
    {
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<T> entities)
    {
        foreach (var e in entities) Items.Add(e);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(T entity)
    {
        var key = _getKey(entity);
        var existing = Items.FirstOrDefault(x => EqualityComparer<TKey>.Default.Equals(_getKey(x), key));
        if (existing != null)
        {
            var idx = Items.IndexOf(existing);
            Items[idx] = entity;
        }
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        foreach (var e in entities) _ = UpdateAsync(e);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TKey id)
    {
        var existing = Items.FirstOrDefault(x => EqualityComparer<TKey>.Default.Equals(_getKey(x), id));
        if (existing != null) Items.Remove(existing);
        return Task.CompletedTask;
    }
}
