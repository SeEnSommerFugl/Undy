using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undy.Data.Repository
{
    public interface IBaseRepository<T, TKey> where T : class where TKey : notnull
    {
        ObservableCollection<T> Items { get; }
        IEnumerable<T> GetAll();
        T? GetById(TKey id);
        void Add(T entity);
        void Update(T entity);
        void Delete(TKey id);
        void UpdateRange(IEnumerable<T> entities);
    }
}
