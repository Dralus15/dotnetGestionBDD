using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    /// <summary>
    /// Classe abstraite regroupant les traitement commun pour accéder au donnée via une base SQLite,
    /// chaque classe étendant <c>AbstactDao</c> correspond à un moyen d'accéder à une table de la base
    /// </summary>
    public abstract class AbstractDao
    {
        /// <summary>
        /// La chaîne de connection pour accéder à la base SQLite, par défaut on créer une connection avec
        /// le fichier Bacchus.SQLite
        /// </summary>
        protected const string CONNECTION_STRING = "Data Source=Bacchus.SQLite";
        
        
        /// <summary>
        /// Le nom de la table pour ce Dao
        /// </summary>
        private readonly string TableName;
        
        /// <summary>
        /// Indique si la clé primaire de cette table est auto-incrémentée
        /// </summary>
        private readonly bool IsAutoIncrement;


        protected AbstractDao(string TableName, bool IsAutoIncrement)
        {
            this.TableName = TableName;
            this.IsAutoIncrement = IsAutoIncrement;
        }

        /// <summary>
        /// Supprime toutes les données de la table associée à ce Dao, si la clé primaire
        /// de la table est auto-incrémentée, l'index d'auto-incrémentation est remis à 0.
        /// </summary>
        /// <exception cref="SQLiteException">Si la connection est interrompue</exception>
        public void Clear()
        {
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand($"DELETE FROM {TableName}; VACUUM;", Connection))
                {
                    Command.ExecuteNonQuery();
                }
                //On reset l'auto-incrémentation si la table à une clé primaire auto-incrémentée
                if (IsAutoIncrement)
                {
                    using (var Command = new SQLiteCommand(
                        $"update sqlite_sequence set seq = 0 where name='{TableName}'", Connection))
                    {
                        Command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Retourne le nombre de ligne de la table associée à ce Dao
        /// </summary>
        /// <returns>Le nombre de ligne de la table</returns>
        /// <exception cref="SQLiteException">Si la connection est interrompue</exception>
        public int Count()
        {
            var Result = 0;
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand($"SELECT count(*) FROM {TableName}", Connection))
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