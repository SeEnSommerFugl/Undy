using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Data.Database;

namespace Undy.Data.Repository
{
    /// <summary>
    /// Data access for StartPage KPI cards.
    /// Uses stored procedures defined in: Undy/Features/SQL/02_ViewsAndProcedures.sql
    /// </summary>
    public class StartPageDBRepository
    {
        public async Task<int> GetPackedTodayAsync(DateOnly today)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_PackedToday", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@Today", SqlDbType.Date).Value = today.ToDateTime(TimeOnly.MinValue);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<int> GetReadyToPickAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_ReadyToPick", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<int> GetPackedTotalAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_PackedTotal", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        /// <summary>
        /// Implements your current definition:
        /// (COUNT(Customers) / SUM(SalesOrder.TotalPrice)).
        /// Returns a formatted string.
        /// </summary>
        public async Task<string> GetAverageCustomerValueAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_AverageCustomerValue", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var raw = await cmd.ExecuteScalarAsync();
            if (raw == null || raw == DBNull.Value) return "0";

            var value = Convert.ToDecimal(raw);
            return value.ToString("0.####");
        }

        public async Task<int> GetWholesaleOnTheWayAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_WholesaleOnTheWay", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<string> GetTotalReturnRateAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_TotalReturnRate", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var raw = await cmd.ExecuteScalarAsync();
            if (raw == null || raw == DBNull.Value) return "0%";

            var rate = Convert.ToDecimal(raw) * 100m;
            return rate.ToString("0.##") + "%";
        }

        public async Task<int> GetOutstandingPaymentsAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_OutstandingPayments", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<int> GetUniqueCustomersAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_UniqueCustomers", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
    }
}
