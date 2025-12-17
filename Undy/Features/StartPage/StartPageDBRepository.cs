using System;
using System.Data;
using System.Globalization;
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

            var raw = await cmd.ExecuteScalarAsync();
            return raw == null || raw == DBNull.Value ? 0 : Convert.ToInt32(raw);
        }

        public async Task<int> GetReadyToPickAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_ReadyToPick", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var raw = await cmd.ExecuteScalarAsync();
            return raw == null || raw == DBNull.Value ? 0 : Convert.ToInt32(raw);
        }

        public async Task<int> GetPackedTotalAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_PackedTotal", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var raw = await cmd.ExecuteScalarAsync();
            return raw == null || raw == DBNull.Value ? 0 : Convert.ToInt32(raw);
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
            return value.ToString("0.####", CultureInfo.InvariantCulture);
        }

        public async Task<decimal> GetAverageOrderValueAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_AverageOrderValue", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var raw = await cmd.ExecuteScalarAsync();
            if (raw == null || raw == DBNull.Value) return 0m;

            return Convert.ToDecimal(raw);
        }

        public async Task<int> GetWholesaleOnTheWayAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_WholesaleOnTheWay", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var raw = await cmd.ExecuteScalarAsync();
            return raw == null || raw == DBNull.Value ? 0 : Convert.ToInt32(raw);
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
            return rate.ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }

        public async Task<int> GetOutstandingPaymentsAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_OutstandingPayments", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var raw = await cmd.ExecuteScalarAsync();
            return raw == null || raw == DBNull.Value ? 0 : Convert.ToInt32(raw);
        }

        public async Task<int> GetUniqueCustomersAsync()
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("dbo.usp_StartPage_UniqueCustomers", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var raw = await cmd.ExecuteScalarAsync();
            return raw == null || raw == DBNull.Value ? 0 : Convert.ToInt32(raw);
        }
    }
}
