using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Undy.Data;

namespace Undy.Data.Repository
{
    public abstract class BaseDBRepository<T, TKey> : IBaseRepository<T, TKey> where T : class where TKey : notnull
    {
        protected readonly ObservableCollection<T> _items;

        public ObservableCollection<T> Items => _items;

        protected BaseDBRepository()
        {
            _items = new ObservableCollection<T>();
        }

        public async Task InitializeAsync()
        {
            var fresh = await QueryAllAsync();
            _items.Clear();
            foreach (var e in fresh)
                _items.Add(e);
        }

        // --------- Template members you implement in each concrete repo ---------

        // SQL templates
        protected abstract string SqlSelectAll { get; }
        protected abstract string SqlSelectById { get; }
        protected abstract string SqlInsert { get; }
        protected abstract string SqlUpdate { get; }
        protected abstract string SqlDeleteById { get; }

        // Mapping and bindings
        protected abstract T Map(IDataRecord r);
        protected abstract void BindId(SqlCommand cmd, TKey id);
        protected abstract void BindInsert(SqlCommand cmd, T entity);
        protected abstract void BindUpdate(SqlCommand cmd, T entity);

        // Key helpers
        protected abstract TKey GetKey(T entity);

        public async Task<T?> GetByIdAsync(TKey id)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(SqlSelectById, con);

            BindId(cmd, id);
            
            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync() ? Map(rd) : null;
        }

        public async Task AddAsync(T entity)
        {
            if (GetKey(entity) is Guid g && g == Guid.Empty)
                throw new InvalidOperationException("Entity key must be set before insert.");

            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(SqlInsert, con);

            BindInsert(cmd, entity);

            var affected = await cmd.ExecuteNonQueryAsync();
            if (affected != 1)
                throw new InvalidOperationException("Insert failed.");

            _items.Add(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(SqlUpdate, con);

            BindUpdate(cmd, entity);

            await cmd.ExecuteNonQueryAsync();
            await ReloadItemsAsync();
        }
        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            var entitiesList = entities.ToList();
            if (!entitiesList.Any()) return;

            using var con = await DB.OpenConnection();
            using var transaction = (SqlTransaction)await con.BeginTransactionAsync();

            try
            {
                foreach (var entity in entitiesList)
                {
                    using var cmd = new SqlCommand(SqlUpdate, con, transaction);
                    BindUpdate(cmd, entity);
                    await cmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                // Only reload once after all updates are complete
                await ReloadItemsAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(TKey id)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(SqlDeleteById, con);

            BindId(cmd, id);

            await cmd.ExecuteNonQueryAsync();

            var existing = _items.FirstOrDefault(x => Equals(GetKey(x), id));
            if (existing != null) 
                _items.Remove(existing);
        }

        // --------------------- Helpers ---------------------

        protected async Task ReloadItemsAsync()
        {
            var fresh = await QueryAllAsync();
            _items.Clear();
            foreach (var e in fresh) 
                _items.Add(e);
        }

        private async Task<List<T>> QueryAllAsync()
        {
            var list = new List<T>();

            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(SqlSelectAll, con);
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync()) list.Add(Map(rd));

            return list;
        }
    }
}
