using System.Data;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public abstract class AbstractDao
    {
        
        private const string DbPath = "Data Source=Bacchus.SQLite";

        protected SQLiteConnection Connection = null;

        protected SQLiteConnection NewConnection()
        {
            if (Connection == null || Connection.State == ConnectionState.Closed)
            {
                Connection = new SQLiteConnection(DbPath);
                Connection.Open();
            }

            return Connection;
        }
    }
}