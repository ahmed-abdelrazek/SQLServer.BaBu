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

            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = $"ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] SET Single_User WITH Rollback Immediate";
                var command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                query = $"USE master RESTORE DATABASE [{sqlConStrBuilder.InitialCatalog}] FROM DISK='{fileName}' WITH REPLACE;";
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                query = $"USE master ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] SET Multi_User";
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Take backup from the database and will overwrite any file with the same name
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

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                var command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                query = $"USE master ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] SET Multi_User";
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                //moves the bakcup file from temp folder to the chosen location and name
                File.Copy(backupfile, fileName, true);
                File.Delete(backupfile);
            }
        }

        /// <summary>
        /// Alters the database Collate
        /// </summary>
        /// <param name="conn">The connection string for the sqlserver database</param>
        /// <param name="collate">the collate like Arabic_CI_AI or something</param>
        public static void AlterCollate(string conn, string collate)
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
                command.ExecuteNonQuery();

                query = $"ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] COLLATE {collate}";
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                query = $"USE master ALTER DATABASE [{sqlConStrBuilder.InitialCatalog}] SET Multi_User";
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
