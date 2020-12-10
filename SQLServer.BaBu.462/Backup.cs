using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace SQLServer.BaBu
{
    public class Backup
    {
        /// <summary>
        /// Restore backup file to database
        /// </summary>
        /// <param name="conn">The connection string for the sqlserver database</param>
        /// <param name="fileName">The full path to the backup file</param>
        public static async Task RestoreAsync(string conn, string fileName)
        {
            var sqlConStrBuilder = new SqlConnectionStringBuilder(conn);

            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = $"ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] SET Single_User WITH Rollback Immediate";
                var command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                query = $"USE master RESTORE DATABASE [{sqlConStrBuilder.InitialCatalog}] FROM DISK='{fileName}' WITH REPLACE;";
                command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                query = $"USE master ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] SET Multi_User";
                command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Take backup from the database and will overwrite any file with the same name
        /// </summary>
        /// <param name="conn">The connection string for the sqlserver database</param>
        /// <param name="fileName">The full path to the backup file</param>
        public static async Task TakeAsync(string conn, string fileName)
        {
            var sqlConStrBuilder = new SqlConnectionStringBuilder(conn);

            using (var connection = new SqlConnection(conn))
            {
                string tempFolder = $"{ Path.GetPathRoot(Environment.SystemDirectory)}temp";
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                string backupfile = $"{tempFolder}\\Backup { DateTime.Now:yyyy-MM-dd hh-mm-ss tt}.bak";

                string query = $"BACKUP DATABASE [{sqlConStrBuilder.InitialCatalog}] TO DISK='{backupfile}'";

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                var command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                query = $"USE master ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] SET Multi_User";
                command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                File.Copy(backupfile, fileName, true);
                File.Delete(backupfile);
            }
        }

        /// <summary>
        /// Alters the database Collate
        /// </summary>
        /// <param name="conn">The connection string for the sqlserver database</param>
        /// <param name="collate">the collate like Arabic_CI_AI or something</param>
        public static async Task AlterCollateAsync(string conn, string collate)
        {
            var sqlConStrBuilder = new SqlConnectionStringBuilder(conn);

            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = $"ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] SET Single_User WITH Rollback Immediate";
                var command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                query = $"ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] COLLATE {collate}";
                command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                query = $"USE master ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] SET Multi_User";
                command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
