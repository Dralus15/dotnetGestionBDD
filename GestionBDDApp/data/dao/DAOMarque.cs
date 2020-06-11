﻿using System;
 using GestionBDDApp.data.model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public class DaoMarque : AbstractDao
    {
        public DaoMarque() : base("Marques", true) { }
        
        public List<Marques> GetAllMarques()
        {
            List<Marques> Marques;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM Marques;", Connection))
                {
                    using (var Reader = Command.ExecuteReader())
                    {
                        Marques = ParseQueryResult(Reader);
                    }
                }
            }

            return Marques;
        }

        private static List<Marques> ParseQueryResult(SQLiteDataReader DataReader)
        {
            var Marques = new List<Marques>();
            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    Marques.Add(new Marques(DataReader.GetInt32(0), DataReader.GetString(1)));
                }
            }

            return Marques;
        }

        public Marques GetMarqueById(int Id)
        {
            Marques Marque = null;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM Marques WHERE RefMarque = @refMarque", Connection))
                {
                    Command.Parameters.AddWithValue("@refMarque", Id);
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            Marque = new Marques(Reader.GetInt32(0), Reader.GetString(1));
                        }
                    }
                }
            }

            return Marque;
        }

        public void Save(Marques Marques)
        {
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(Connection))
                {
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
            }
        }

        public void Delete(int Id)
        {
            var UseCount = DaoRegistery.GetInstance.DaoArticle.CountArticleOfBrand(Id);
            if (UseCount > 0)
            {
                string Error;
                if (UseCount == 1)
                {
                    Error = "Cette marque est utilisée par 1 article, veuilliez supprimer l'article utilisant cette marque avant de la supprimer.";
                }
                else
                {
                    Error = String.Format(
                        "Cette marque est utilisée par {0} articles, veuilliez supprimer les articles utilisant cette marque avant de la supprimer.",
                        UseCount);
                }
                throw new ArgumentException(Error);
            }
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("DELETE FROM Marques WHERE RefMarque = @ref", Connection) )
                {
                    Command.Parameters.AddWithValue("@ref", Id);
                    Command.ExecuteNonQuery();
                }
            }
        }
        
        public List<Marques> GetBrandByName(string BrandName)
        {
            List<Marques> Brand;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM Marques WHERE Nom = '@name'", Connection))
                {
                    Command.Parameters.AddWithValue("@name", BrandName);
                    using (var Reader = Command.ExecuteReader())
                    {
                        Brand = ParseQueryResult(Reader);
                    }
                }
            }

            return Brand;
        }
    }
}
