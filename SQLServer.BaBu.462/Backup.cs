using System.Data.SqlClient;
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
        public static async Task Restore(string conn, string fileName)
        {
            var sqlConStrBuilder = new SqlConnectionStringBuilder(conn);
            var database = sqlConStrBuilder.InitialCatalog;
            string query = null;
            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                query = $"ALTER DATABASE [{database}] SET Single_User WITH Rollback Immediate";
                using (var command = new SqlCommand(query, connection))
                {
                    await Task.Run(() =>
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    });
                }
                query = $"USE master RESTORE DATABASE [{database}] FROM DISK='{fileName}' WITH  FILE = 1,  NOUNLOAD,  STATS = 10";
                using (var command = new SqlCommand(query, connection))
                {
                    await Task.Run(() =>
                    {
                        command.ExecuteNonQuery();
                    });
                }
                query = $"USE master ALTER DATABASE [{database}] SET Multi_User";
                using (var command = new SqlCommand(query, connection))
                {
                    await Task.Run(() =>
                    {
                        command.ExecuteNonQuery();
                    });
                }
            }
        }

        /// <summary>
        /// Take backup from database
        /// </summary>
        /// <param name="conn">The connection string for the sqlserver database</param>
        /// <param name="fileName">The full path to the backup file</param>
        public static async Task Take(string conn, string fileName)
        {
            var sqlConStrBuilder = new SqlConnectionStringBuilder(conn);
            using (var connection = new SqlConnection(conn))
            {
                var query = $"BACKUP DATABASE [{sqlConStrBuilder.InitialCatalog}] TO DISK='{fileName}'";
                using (var command = new SqlCommand(query, connection))
                {
                    await Task.Run(() =>
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    });
                }
            }
        }
    }
}
