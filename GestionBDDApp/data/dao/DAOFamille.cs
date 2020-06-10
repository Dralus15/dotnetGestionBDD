﻿using GestionBDDApp.data.model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public class DAOFamille : AbstractDao
    {
        public DAOFamille() : base("Familles") { }

        public List<Familles> GetAllFamilles()
        {
            List<Familles> Familles;

            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM Articles;", Connection))
                {
                    using (var Reader = Command.ExecuteReader())
                    {
                        Familles = ParseQueryResult(Reader);
                    }
                }
            }

            return Familles;
        }

        private static List<Familles> ParseQueryResult(SQLiteDataReader DataReader)
        {
            List<Familles> Familles = new List<Familles>();
            if (!DataReader.HasRows) return Familles;
            while (DataReader.Read())
            {
                Familles.Add(new Familles(DataReader.GetInt32(0), DataReader.GetString(1)));
            }

            return Familles;
        }


        public Familles GetFamilleById(int Id)
        {
            Familles Famille = null;

            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM Familles WHERE RefFamille = @refFamille", Connection))
                {
                    
                    Command.Parameters.AddWithValue("@refFamille", Id);
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            Famille = new Familles(Reader.GetInt32(0), Reader.GetString(1));
                        }
                    }
                }
            }

            return Famille;
        }

        public List<Familles> GetFamilleByName(string FamilleName)
        {
            List<Familles> Familles;

            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM Familles WHERE Nom = '@name'", Connection))
                {
                    Command.Parameters.AddWithValue("@name", FamilleName);
                    using (var Reader = Command.ExecuteReader())
                    {
                        Familles = ParseQueryResult(Reader);
                    }
                }
            }

            return Familles;
        }

        public void save(Familles Famille)
        {
            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(Connection))
                {
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
            }
        }

        public void delete(int Id)
        {
            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("DELETE FROM Articles WHERE RefArticle = @ref", Connection))
                {
                    Command.Parameters.AddWithValue("@ref", Id);
                    Command.ExecuteNonQuery();
                }
            }
        }
    }
}
