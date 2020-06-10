using System;
using System.Data;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public abstract class AbstractDao
    {
        private readonly string BaseName;

        protected const string ConnectionString = "Data Source=Bacchus.SQLite";

        public AbstractDao(String BaseName)
        {
            this.BaseName = BaseName;
        }

        public void clear()
        {
            using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (SQLiteCommand Command = new SQLiteCommand(String.Format("DELETE FROM {0}; VACUUM;", BaseName)))
                {
                    Command.ExecuteNonQuery();
                }
            }
        }
    }
}