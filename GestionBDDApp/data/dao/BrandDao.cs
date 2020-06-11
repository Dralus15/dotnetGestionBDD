﻿using System;
 using GestionBDDApp.data.model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    /// <summary>
    /// Classe du Dao pour les marques, elle permet de faire des traitements sur la table Marques.
    /// </summary>
    public class BrandDao : AbstractDao
    {
        /// <summary>
        /// Instancie le Dao des marques.
        /// </summary>
        public BrandDao() : base("Marques", true) { }

        /// <summary>
        /// Récupère toutes les marques dans la base et les retourne dans une <b>List'&lt;'Marques'&gt;'</b>.
        /// </summary>
        public List<Marques> GetAllMarques()
        {
            List<Marques> Marques;

            // On se connecte à la base de données pour envoyer la requête.
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();

                // Création de la requête.
                using (var Command = new SQLiteCommand("SELECT * FROM Marques;", Connection))
                {
                    // On récupère la requête et on la parse pour obtenir une liste des marques.
                    using (var Reader = Command.ExecuteReader())
                    {
                        Marques = ParseQueryResult(Reader);
                    }
                }
            }

            return Marques;
        }

        /// <summary>
        /// Parse le resultat de la requête SQL pour le retourner dans une <b>List'&lt;'Marques'&gt;'</b>.
        /// </summary>
        /// <param name="DataReader">Le résultat de la requête SQL.</param>
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

        /// <summary>
        /// Cherche une marque par son id et la retourne.
        /// </summary>
        /// <param name="Id">id de la marque recherchée.</param>
        public Marques GetMarqueById(int Id)
        {
            Marques Marque = null;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "SELECT * FROM Marques WHERE RefMarque = @refMarque", Connection))
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

        /// <summary>
        /// Sauvegarde la marque dans la base (si elle existe déjà, alors elle est mise à jour).
        /// </summary>
        /// <param name="Marques">La marque à sauvegarder.</param>
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

        /// <summary>
        /// Supprime la marque dans la base.
        /// Si la marque est utilisée par des articles alors un message d'erreur est renvoyé.
        /// </summary>
        /// <param name="Id">Id de la marque à supprimer.</param>
        public void Delete(int Id)
        {
            var UseCount = DaoRegistry.GetInstance.ArticleDao.CountArticleOfBrand(Id);

            // Vérifie si la marque est utilisée par un article, et renvoie un message d'erreur si c'est le cas.
            if (UseCount > 0)
            {
                string Error;
                if (UseCount == 1)
                {
                    Error = "Cette marque est utilisée par 1 article, veuilliez supprimer l'article utilisant " 
                            + "cette marque avant de la supprimer.";
                }
                else
                {
                    Error =
                        $"Cette marque est utilisée par {UseCount} articles, veuilliez supprimer les articles utilisant " +
                        "cette marque avant de la supprimer.";
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

        /// <summary>
        /// Cherche une marque par son nom et la retourne.
        /// </summary>
        /// <param name="BrandName">Le nom de la marque.</param>
        public List<Marques> GetBrandByName(string BrandName)
        {
            List<Marques> Brand;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM Marques WHERE Nom = @name", Connection))
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
