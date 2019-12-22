using System;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Data.SqlClient;

namespace AzureSqlAppIdentityAuthTool
{
    public static class Program
    {
        public class Options
        {
            [Option('c', "connectionstring", Required = true, HelpText = "The SQL server connection string to use. Should use Azure AD authentication.")]
            public string ConnectionString { get; set; } = null!; // Will result in NotParsed if missing.

            [Option('i', "identity", Required = true, HelpText = "The managed identity to authorize")]
            public string IdentityName { get; set; } = null!; // Will result in NotParsed if missing.

            [Option("no-datareader", Default = false, Required = false, HelpText = "Do not add the identity to the 'db_datareader' role.")]
            public bool NoDataReader { get; set; }

            [Option("no-datawriter", Default = false, Required = false, HelpText = "Do not add the identity to the 'db_datawriter' role.")]
            public bool NoDataWriter { get; set; }

            [Option("no-ddladmin", Default = false, Required = false, HelpText = "Do not add the identity to the 'db_ddladmin' role.")]
            public bool NoDdlAdmin { get; set; }
        }

        public static async Task<int> Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);

            if (result is NotParsed<Options>)
            {
                return -1;
            }

            if (!(result is Parsed<Options> parsed))
            {
                // Should not happen.
                throw new InvalidOperationException("Could not parse command line.");
            }

            var options = parsed.Value;

            using var connection = new SqlConnection(options.ConnectionString);
            await connection.OpenAsync();

            await CreateUserAsync(connection, options.IdentityName);

            if (!options.NoDataReader)
            {
                await AddUserToRoleAsync(connection, options.IdentityName, "db_datareader");
            }

            if (!options.NoDataWriter)
            {
                await AddUserToRoleAsync(connection, options.IdentityName, "db_datawriter");
            }

            if (!options.NoDdlAdmin)
            {
                await AddUserToRoleAsync(connection, options.IdentityName, "db_ddladmin");
            }

            return 0;
        }

        private static async Task CreateUserAsync(SqlConnection connection, string identityName)
        {
            const int UserAlreadyExistsErrorNumber = 15023;

            using var createUser = connection.CreateCommand();
            createUser.CommandText = "CREATE USER [" + identityName + "] FROM EXTERNAL PROVIDER";

            try
            {
                await createUser.ExecuteNonQueryAsync();
            }
            catch (SqlException ex) when (ex.Number == UserAlreadyExistsErrorNumber)
            {
                // User already exists, continue.
            }
        }

        private static async Task AddUserToRoleAsync(SqlConnection connection, string identityName, string role)
        {
            using var alterRole = connection.CreateCommand();
            alterRole.CommandText = "ALTER ROLE [" + role + "] ADD MEMBER [" + identityName + "];";

            await alterRole.ExecuteNonQueryAsync();
        }
    }
}
