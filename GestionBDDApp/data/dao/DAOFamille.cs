﻿using System;
 using GestionBDDApp.data.model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    /// <summary>
    /// Classe du Dao pour les familles, elle permet de faire des traitements sur la table Familles.
    /// </summary>
    public class DaoFamille : AbstractDao
    {
        /// <summary>
        /// Instancie le Dao des familles.
        /// </summary>
        public DaoFamille() : base("Familles", true) { }

        /// <summary>
        /// Récupère toutes les familles dans la base et les retourne dans une <b>List'<'Familles'>'</b>.
        /// </summary>
        public List<Familles> GetAllFamilles()
        {
            List<Familles> Familles;

            // On se connecte à la base de données pour envoyer la requête.
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();

                // Création de la requête.
                using (var Command = new SQLiteCommand("SELECT * FROM Familles;", Connection))
                {
                    // On récupère la requête et on la parse pour obtenir une liste des familles.
                    using (var Reader = Command.ExecuteReader())
                    {
                        Familles = ParseQueryResult(Reader);
                    }
                }
            }

            return Familles;
        }

        /// <summary>
        /// Parse le resultat de la requête SQL pour le retourner dans une <b>List'<'Familles'>'</b>.
        /// </summary>
        /// <param name="DataReader">Le résultat de la requête SQL.</param>
        private static List<Familles> ParseQueryResult(SQLiteDataReader DataReader)
        {
            var Familles = new List<Familles>();
            if (!DataReader.HasRows) return Familles;
            while (DataReader.Read())
            {
                Familles.Add(new Familles(DataReader.GetInt32(0), DataReader.GetString(1)));
            }

            return Familles;
        }

        /// <summary>
        /// Cherche une famille par son id et la retourne.
        /// </summary>
        /// <param name="Id">id de la famille recherchée.</param>
        public Familles GetFamilleById(int Id)
        {
            Familles Famille = null;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "SELECT * FROM Familles WHERE RefFamille = @refFamille", Connection))
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

        /// <summary>
        /// Cherche une famille par son nom et la retourne.
        /// </summary>
        /// <param name="FamilleName">Nom de la famille recherchée.</param>
        public List<Familles> GetFamilleByName(string FamilleName)
        {
            List<Familles> Familles;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
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

        /// <summary>
        /// Sauvegarde la famille dans la base (si elle existe déjà, alors elle est mise à jour).
        /// </summary>
        /// <param name="Famille">La famille à sauvegarder.</param>
        public void Save(Familles Famille)
        {
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
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
                        Command.CommandText = @"UPDATE Familles SET Nom = @name WHERE RefFamille = @refFamille";
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

        /// <summary>
        /// Supprime la famille dans la base.
        /// Si la famille est utilisée par des sous-familles alors un message d'erreur est renvoyé.
        /// </summary>
        /// <param name="Id">Id de la famille à supprimer.</param>
        public void Delete(int Id)
        {
            var UseCount = DaoRegistery.GetInstance.DaoSousFamille.CountSubFamilyOfFamily(Id);

            // Vérifie si la famille est utilisée par une sous-famille, et renvoie un message d'erreur si c'est le cas.
            if (UseCount > 0)
            {
                string Error;
                if (UseCount == 1)
                {
                    Error = "Cette famille est utilisée par 1 sous-famille, veuilliez supprimer la sous-famille " 
                            + "utilisant cette famille avant de la supprimer.";
                }
                else
                {
                    Error = String.Format(
                        "Cette famille est utilisée par {0} sous-familles, veuilliez supprimer les sous-familles " 
                        + "utilisant cette famille avant de la supprimer.",
                        UseCount);
                }
                throw new ArgumentException(Error);
            }
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("DELETE FROM Familles WHERE RefFamille = @ref", Connection))
                {
                    Command.Parameters.AddWithValue("@ref", Id);
                    Command.ExecuteNonQuery();
                }
            }
        }
    }
}
