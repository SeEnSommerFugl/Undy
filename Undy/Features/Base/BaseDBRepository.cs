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
        protected virtual string SqlSelectAll { get; }
        protected virtual string SqlSelectById { get; }
        protected virtual string SqlInsert { get; }
        protected virtual string SqlUpdate { get; }
        protected virtual string SqlDeleteById { get; }

        // Mapping and bindings
        protected abstract T Map(IDataRecord r);
        protected virtual void BindId(SqlCommand cmd, TKey id)
        {

        }
        protected virtual void BindInsert(SqlCommand cmd, T entity)
        {

        }
        protected virtual void BindUpdate(SqlCommand cmd, T entity)
        {

        }

        // Key helpers
        protected abstract TKey GetKey(T entity);

        public async Task<T?> GetByIdAsync(TKey id)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(SqlSelectById, con);
            cmd.CommandType = CommandType.StoredProcedure;

            BindId(cmd, id);

            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync() ? Map(rd) : null;
        }
        public async Task<List<T>> GetByIdsAsync(IEnumerable<TKey> ids)
        {
            var list = new List<T>();
            var idList = ids.ToList();

            if (idList.Count == 0)
                return list;

            using var con = await DB.OpenConnection();

            foreach (var id in idList)
            {
                using var cmd = new SqlCommand(SqlSelectById, con);
                cmd.CommandType = CommandType.StoredProcedure;

                BindId(cmd, id);

                using var rd = await cmd.ExecuteReaderAsync();
                while (await rd.ReadAsync())
                {
                    list.Add(Map(rd));
                }
            }

            return list;
        }

        public async Task AddAsync(T entity)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(SqlInsert, con);
            cmd.CommandType = CommandType.StoredProcedure;

            BindInsert(cmd, entity);

            var affected = await cmd.ExecuteNonQueryAsync();

            await ReloadItemsAsync();
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            var entitiesList = entities.ToList();
            if (!entitiesList.Any()) return;

            using var con = await DB.OpenConnection();
            using var transaction = (SqlTransaction)await con.BeginTransactionAsync();

            try
            {
                foreach (var entity in entitiesList)
                {
                    using var cmd = new SqlCommand(SqlInsert, con, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;

                    BindInsert(cmd, entity);

                    await cmd.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            await ReloadItemsAsync();
        }


        public async Task UpdateAsync(T entity)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(SqlUpdate, con);
            cmd.CommandType = CommandType.StoredProcedure;

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
                    cmd.CommandType = CommandType.StoredProcedure;

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
            cmd.CommandType = CommandType.StoredProcedure;

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
            cmd.CommandType = CommandType.Text;
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync()) list.Add(Map(rd));

            return list;
        }
    }
}
