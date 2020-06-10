using System;
using System.Data;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public abstract class AbstractDao
    {
        private readonly string BaseName;

        private const string DbPath = "Data Source=Bacchus.SQLite";

        protected SQLiteConnection Connection = null;

        public AbstractDao(String BaseName)
        {
            this.BaseName = BaseName;
        }

        protected SQLiteConnection NewConnection()
        {
            if (Connection == null || Connection.State == ConnectionState.Closed)
            {
                Connection = new SQLiteConnection(DbPath);
                Connection.Open();
            }

            return Connection;
        }

        public void clear()
        {
            new SQLiteCommand(String.Format("DELETE FROM {0}; VACUUM;", BaseName), NewConnection()).ExecuteNonQuery();
        }
    }
}