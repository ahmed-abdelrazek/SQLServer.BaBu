using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace SQLServer.BaBu
{
    public class Bulk
    {
        /// <summary>
        /// Execute SQL Script from string
        /// </summary>
        /// <param name="connection">The connection string for the sqlserver database</param>
        /// <param name="script">The SQL string to execute</param>
        public static async Task FromScriptAsync(string connection, string script)
        {
            await Task.Run(() =>
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    Server db = new Server(new ServerConnection(conn));
                    string nscript = script.Trim();

                    db.ConnectionContext.ExecuteNonQuery(nscript);
                }
            });
        }

        /// <summary>
        /// Execute SQL Script from file
        /// </summary>
        /// <param name="connection">The connection string for the sqlserver database</param>
        /// <param name="fileName">The full path to the file that have the SQL string to execute</param>
        public static async Task FromFileAsync(string connection, string fileName)
        {
            await Task.Run(() =>
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    Server db = new Server(new ServerConnection(conn));
                    string script = File.ReadAllText(fileName);
                    string nscript = script.Trim();

                    if (!string.IsNullOrWhiteSpace(nscript))
                    {
                        db.ConnectionContext.ExecuteNonQuery(nscript);
                    }
                }
            });
        }
    }
}
