using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.IO;

namespace SQLServer.BaBu
{
    public class Bulk
    {
        /// <summary>
        /// Execute SQL Script from string
        /// </summary>
        /// <param name="connection">The connection string for the sqlserver database</param>
        /// <param name="script">The SQL string to execute</param>
        public static void FromScript(string connection, string script)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                Server db = new Server(new ServerConnection(conn));
                string nscript = script.Trim();
                db.ConnectionContext.ExecuteNonQuery(nscript);
            }
        }

        /// <summary>
        /// Execute SQL Script from file
        /// </summary>
        /// <param name="connection">The connection string for the sqlserver database</param>
        /// <param name="fileName">The full path to the file that have the SQL string to execute</param>
        public static void FromFile(string connection, string fileName)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                Server db = new Server(new ServerConnection(conn));
                string script = File.ReadAllText(fileName).Trim();
                db.ConnectionContext.ExecuteNonQuery(script);
            }
        }
    }
}