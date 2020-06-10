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
            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(String.Format("DELETE FROM {0}; VACUUM;", BaseName), Connection))
                {
                    Command.ExecuteNonQuery();
                }
            }
        }

        public int count()
        {
            int Result = 0;
            using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (SQLiteCommand Command = new SQLiteCommand(String.Format("SELECT count(*) FROM {0}", BaseName), Connection))
                {
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            Result = Reader.GetInt32(0);
                        }
                    }
                }
            }

            return Result;
        }
    }
}