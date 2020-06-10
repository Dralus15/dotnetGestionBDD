using GestionBDDApp.data.model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public class DAOSousFamille : AbstractDao
    {
        private readonly DAOFamille DaoFamille;

        public DAOSousFamille(DAOFamille DaoFamille) : base("SousFamilles")
        {
            this.DaoFamille = DaoFamille;
        }

        public List<SousFamilles> GetAllSousFamilles()
        {
            return ParseQueryResult(new SQLiteCommand("SELECT * FROM SousFamilles", NewConnection()).ExecuteReader());
        }

        private List<SousFamilles> ParseQueryResult(SQLiteDataReader DataReader)
        {
            List<SousFamilles> SousFamilles = new List<SousFamilles>();
            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    DAOFamille DaoFamille = new DAOFamille();
                    Familles Famille = this.DaoFamille.GetFamilleById(DataReader.GetInt32(1));
                    SousFamilles.Add(new SousFamilles(DataReader.GetInt32(0), Famille, DataReader.GetString(2)));
                }
            }

            return SousFamilles;
        }

        public List<SousFamilles> getSousFamilleByName(string Name)
        {
            var SqLiteCommand = new SQLiteCommand(NewConnection());
            SqLiteCommand.CommandText = "SELECT * FROM SousFamilles WHERE Nom = @name";
            SqLiteCommand.Parameters.AddWithValue("@name", Name);
            return ParseQueryResult(SqLiteCommand.ExecuteReader());
        }       
        
        public SousFamilles getSousFamilleById(int id)
        {
            var SqLiteCommand = new SQLiteCommand(NewConnection());
            SqLiteCommand.CommandText = "SELECT * FROM SousFamilles WHERE RefSousFamille = @refSousFamille";
            SqLiteCommand.Parameters.AddWithValue("@refSousFamille", id);
            var SousFamilles = ParseQueryResult(SqLiteCommand.ExecuteReader());
            //SqLiteCommand.Connection.Close();
            if (SousFamilles.Count == 0)
            {
                return null;
            }
            return SousFamilles[0];
        }

        public void save(SousFamilles SousFamille)
        {
            var Connection = NewConnection();
            var Command = new SQLiteCommand(Connection);
            if (SousFamille.Id == null)
            {
                Command.CommandText = "INSERT INTO SousFamilles(RefFamille, Nom) VALUES (@refFamille, @name)";
            }
            else
            {
                Command.CommandText = @"UPDATE SousFamilles SET RefFamille='@refFamille', Nom='@name' WHERE RefSousFamille = @refSousFamille";
                Command.Parameters.AddWithValue("@refSousFamille", SousFamille.Id);
            }
        
            Command.Parameters.AddWithValue("@refFamille", SousFamille.Famille.Id);
            Command.Parameters.AddWithValue("@name", SousFamille.Nom);
            Command.ExecuteNonQuery();
            if (SousFamille.Id == null)
            {
                SousFamille.Id = (int) Connection.LastInsertRowId;
            }
            //Command.Connection.Close();
        }

        public void delete(int Id)
        {
            var Connection = NewConnection();
            var Command = new SQLiteCommand(Connection);
            Command.CommandText = "DELETE FROM SousFamilles WHERE RefSousFamille = @ref";
            Command.Parameters.AddWithValue("@ref", Id);
            Command.ExecuteNonQuery();
            //Command.Connection.Close();
        }
    }
}
