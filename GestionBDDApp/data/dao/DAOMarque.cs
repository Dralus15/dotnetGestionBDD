using GestionBDDApp.data.model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public class DAOMarque : AbstractDao
    {
        public DAOMarque() : base("Marques") { }
        
        public List<Marques> GetAllMarques()
        {
            return ParseQueryResult(new SQLiteCommand( "SELECT * FROM Marques", NewConnection()).ExecuteReader());
        }

        private static List<Marques> ParseQueryResult(SQLiteDataReader DataReader)
        {
            List<Marques> Marques = new List<Marques>();
            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    Marques.Add(new Marques(DataReader.GetInt32(0),
                        DataReader.GetString(1)));
                }
            }

            return Marques;
        }

        public Marques GetMarqueById(int Id)
        {
            var SqLiteCommand = new SQLiteCommand(NewConnection());
            SqLiteCommand.CommandText = "SELECT * FROM Marques WHERE RefMarque = @refMarque";
            SqLiteCommand.Parameters.AddWithValue("@refMarque", Id);
            var Marques = ParseQueryResult(SqLiteCommand.ExecuteReader());
            if (Marques.Count == 0)
            {
                return null;
            }
            return Marques[0];
        }
        
        public List<Marques> GetMarqueByName(string MarqueName)
        {
            var SqLiteCommand = new SQLiteCommand(NewConnection());
            SqLiteCommand.CommandText = "SELECT * FROM Marques WHERE Nom = @name";
            SqLiteCommand.Parameters.AddWithValue("@name", MarqueName);
            return ParseQueryResult(SqLiteCommand.ExecuteReader());
        }

        public void save(Marques Marques)
        {
            var Command = new SQLiteCommand(NewConnection());
            if (Marques.Id == null)
            {
                Command.CommandText = "INSERT INTO Marques(Nom) VALUES (@name)";
            }
            else
            {
                Command.CommandText = @"UPDATE Marques SET Nom=@name WHERE RefMarque = @refMarque";
                Command.Parameters.AddWithValue("@refMarque", Marques.Id);
            }
            
            Command.Parameters.AddWithValue("@name", Marques.Nom);
            Command.ExecuteNonQuery();
            if (Marques.Id == null)
            {
                Marques.Id = (int) Connection.LastInsertRowId;
            }
        }

        public void delete(Marques Marque)
        {

        }

    }
}
