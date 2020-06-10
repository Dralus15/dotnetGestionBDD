using GestionBDDApp.data.model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public class DAOFamille : AbstractDao
    {
        public DAOFamille() : base("Familles") { }

        public List<Familles> GetAllFamilles()
        {
            return ParseQueryResult(new SQLiteCommand("SELECT * FROM Familles;",
                NewConnection()).ExecuteReader());
        }

        private static List<Familles> ParseQueryResult(SQLiteDataReader DataReader)
        {
            List<Familles> Familles = new List<Familles>();
            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    Familles.Add(new Familles(DataReader.GetInt32(0), DataReader.GetString(1)));
                }
            }

            return Familles;
        }


        public Familles GetFamilleById(int Id)
        {
            Familles Famille;
            SQLiteCommand Requete = new SQLiteCommand("SELECT * FROM Familles WHERE RefFamille = @refFamille", NewConnection());
            Requete.Parameters.AddWithValue("@refFamille", Id);
            SQLiteDataReader DataReader = Requete.ExecuteReader();

            if (DataReader.HasRows)
            {
                DataReader.Read();
                Famille = new Familles(DataReader.GetInt32(0), DataReader.GetString(1));
                return Famille;
            }
            else
            {
                return null;
            }
        }

        public List<Familles> GetFamilleByName(string FamilleName)
        {
            var SqLiteCommand = new SQLiteCommand(NewConnection());
            SqLiteCommand.CommandText = "SELECT * FROM Familles WHERE Nom = '@name'";
            SqLiteCommand.Parameters.AddWithValue("@name", FamilleName);
            return ParseQueryResult(SqLiteCommand.ExecuteReader());
        }

        public void save(Familles Famille)
        {
            var Connection = NewConnection();
            var Command = new SQLiteCommand(Connection);
            if (Famille.Id == null)
            {
                Command.CommandText = "INSERT INTO Familles(Nom) VALUES (@name)";
            }
            else
            {
                Command.CommandText = @"UPDATE Familles SET Nom='@name' WHERE RefFamille = @refFamille";
                Command.Parameters.AddWithValue("@refFamille", Famille.Id);
            }
        
            Command.Parameters.AddWithValue("@name", Famille.Nom);
            Command.ExecuteNonQuery();
            if (Famille.Id == null)
            {
                Famille.Id = (int) Connection.LastInsertRowId;
            }
        }

        public void delete(int Id)
        {
            var Connection = NewConnection();
            var Command = new SQLiteCommand(Connection);
            Command.CommandText = "DELETE FROM Familles WHERE RefFamille = @ref";
            Command.Parameters.AddWithValue("@ref", Id);
            Command.ExecuteNonQuery();

        }
    }
}
