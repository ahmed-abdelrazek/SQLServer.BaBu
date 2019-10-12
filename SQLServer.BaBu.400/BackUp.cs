using System;
using System.Data.SqlClient;
using System.IO;

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
                //creates the backup file in your windows root path in the temp folder to avoid any access issues normally C:\temp\
                string tempFolder = $"{ Path.GetPathRoot(Environment.SystemDirectory)}temp";
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }
                string backupfile = $"{tempFolder}\\Backup { DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.bak";

                string query = $"BACKUP DATABASE [{sqlConStrBuilder.InitialCatalog}] TO DISK='{backupfile}'";

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                //moves the bakcup file from temp folder to the chosen location and name
                File.Move(backupfile, fileName);
            }
        }
    }
}
