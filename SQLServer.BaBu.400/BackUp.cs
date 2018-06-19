using System.Data.SqlClient;

namespace SQLServer.BaBu
{
    public class Backup
    {
        /// <summary>
        /// Restore backup file to database
        /// </summary>
        /// <param name="conn">The connection string for the sqlserver database</param>
        /// <param name="fileName">The full path to the backup file</param>
        public static void Restore(string conn, string fileName)
        {
            var sqlConStrBuilder = new SqlConnectionStringBuilder(conn);
            var database = sqlConStrBuilder.InitialCatalog;
            string query = null;
            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                query = $"ALTER DATABASE [{database}] SET Single_User WITH Rollback Immediate";
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                query = $"USE master RESTORE DATABASE [{database}] FROM DISK='{fileName}' WITH REPLACE;";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
                query = $"USE master ALTER DATABASE [{database}] SET Multi_User";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Take backup from database
        /// </summary>
        /// <param name="conn">The connection string for the sqlserver database</param>
        /// <param name="fileName">The full path to the backup file</param>
        public static void Take(string conn, string fileName)
        {
            var sqlConStrBuilder = new SqlConnectionStringBuilder(conn);
            using (var connection = new SqlConnection(conn))
            {
                var query = $"BACKUP DATABASE [{sqlConStrBuilder.InitialCatalog}] TO DISK='{fileName}'";
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
