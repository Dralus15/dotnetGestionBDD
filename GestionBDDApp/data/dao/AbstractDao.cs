using System;
using System.Data;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public abstract class AbstractDao
    {
        private readonly string BaseName;
        private readonly bool IsAutoIncrement;

        protected const string CONNECTION_STRING = "Data Source=Bacchus.SQLite";

        protected AbstractDao(string BaseName, bool IsAutoIncrement)
        {
            this.BaseName = BaseName;
            this.IsAutoIncrement = IsAutoIncrement;
        }

        public void Clear()
        {
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand($"DELETE FROM {BaseName}; VACUUM;", Connection))
                {
                    Command.ExecuteNonQuery();
                }
                
                if (IsAutoIncrement)
                {
                    using (var Command = new SQLiteCommand(
                        $"update sqlite_sequence set seq = 0 where name='{BaseName}'", Connection))
                    {
                        Command.ExecuteNonQuery();
                    }
                }
            }
        }

        public int Count()
        {
            var Result = 0;
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand($"SELECT count(*) FROM {BaseName}", Connection))
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