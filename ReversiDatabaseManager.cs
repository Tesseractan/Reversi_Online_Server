using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Reversi_Online_Server_1._1
{
    static class ReversiDatabaseManager
    {
        static ReversiDatabase database;
        static ReversiDatabaseManager()
        {
            database = new ReversiDatabase();
        }
        public static ReversiDatabase Database => database;
    }
    class ReversiDatabase
    {
        const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=c:\users\user\source\repos\Reversi_Online_Server_1.1\Reversi_Online_Server_1.1\Reversi.mdf;Integrated Security=True";
        private SqlConnection connection = new SqlConnection(connectionString);
        public  ReversiDatabase()
        {
            connection.Open();
        }
        ~ReversiDatabase()
        {
            this.connection.Close();
        }
        public SqlConnection Connection => this.connection;
    }
}
