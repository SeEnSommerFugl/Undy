using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Undy.Data
{
    internal class DB
    {
        /// <summary>
        /// Config builder for connection string, gets the "appsettings.json", with optional to false; it must be provided.
        /// "reloadOnChange" to false, means, no hot reload when changing the connection string.
        /// appsettings.json contains no secrets
        /// appsettings.Local.json contains secrets, but is in .gitignore so nobodys logins are shared.
        /// </summary>
        private static readonly IConfigurationRoot _config =
            new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        /// <summary>
        /// Encapsulaped connection string, for privacy concerns
        /// Gets the connection string for the default database connection.
        /// </summary>
        protected static string ConnectionString =>

            _config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing 'DefaultConnection'.");

        /// <summary>
        /// Opens an connection with the given connection string <see cref="SqlConnection()"/>, 
        /// which uses <param name="ConnectionString"></param>
        /// as a parameter input.
        /// </summary>
        /// 
        /// <returns>
        /// Returns the open connection with the connection string attached, to unsure proper encapsulation and privacy.
        /// </returns>
        internal static async Task<SqlConnection> OpenConnection()
        {
            var con = new SqlConnection(ConnectionString);
            await con.OpenAsync();
            return con;
        }
    }
}
