﻿using System;
 using GestionBDDApp.data.model;
using System.Collections.Generic;
 using System.Data.Common;
 using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    /// <summary>
    /// Classe du Dao pour les sous-familles, elle permet de faire des traitements sur la table SousFamilles
    /// </summary>
    public class DaoSousFamille : AbstractDao
    {
        /// <summary>
        /// Le Dao de la famille parent des sous-familles
        /// </summary>
        private readonly DaoFamille DaoFamille;

        /// <summary>
        /// Instancie le Dao des sous-familles, il faut le Dao de la famille parent
        /// </summary>
        /// <param name="DaoFamille">Dao de la famille</param>
        public DaoSousFamille(DaoFamille DaoFamille) : base("SousFamilles", true)
        {
            this.DaoFamille = DaoFamille;
        }

        /// <summary>
        /// Récupère toutes les sous-familles dans la base et les retourne dans une <b>List'<'SousFamilles'>'</b>
        /// </summary>
        public List<SousFamilles> GetAllSousFamilles()
        {
            List<SousFamilles> SousFamilles;

            // On se connecte à la base de donnée pour envoyer la requête et on récupère la réponse dans une <b>List'<'Marques'>'</b>
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();

                // Création de la requête
                using (var Command = new SQLiteCommand("SELECT * FROM SousFamilles", Connection))
                {
                    // On récupère la requête et on la parse pour obtenir une liste des sous-familles 
                    using (var Reader = Command.ExecuteReader())
                    {
                        SousFamilles = ParseQueryResult(Reader);
                    }
                }
            }

            return SousFamilles;
        }

        /// <summary>
        /// Parse le resultat de la requête SQL pour le retourner dans une <b>List'<'SousFamilles'>'</b>
        /// </summary>
        /// <param name="DataReader">Le réultat de la requête SQL</param>
        private List<SousFamilles> ParseQueryResult(DbDataReader DataReader)
        {
            var SubFamilyRed = new List<SousFamilles>();
            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    var Family = DaoFamille.GetFamilleById(DataReader.GetInt32(1));
                    SubFamilyRed.Add(new SousFamilles(DataReader.GetInt32(0), Family, DataReader.GetString(2)));
                }
            }

            return SubFamilyRed;
        }

        /// <summary>
        /// Cherche la sous-famille par id et la retourne
        /// </summary>
        /// <param name="Id">id de la sous-famille recherchée</param>
        public SousFamilles GetSousFamilleById(int Id)
        {
            SousFamilles SubFamilyFound = null;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand( "SELECT * FROM SousFamilles WHERE RefSousFamille = @refSousFamille", Connection))
                { 
                    Command.Parameters.AddWithValue("@refSousFamille", Id);
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            var Famille = DaoFamille.GetFamilleById(Reader.GetInt32(1));
                            SubFamilyFound = new SousFamilles(Reader.GetInt32(0), Famille, Reader.GetString(2));
                        }
                    }
                }
            }

            return SubFamilyFound;
        }

        /// <summary>
        /// Sauvegarde la sous-famille dans la base (si elle existe déjà, alors elle est mise à jour)
        /// </summary>
        /// <param name="SousFamille">Sous-famille à sauvegarder</param>
        public void Save(SousFamilles SousFamille)
        {
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(Connection))
                {
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
                }
            }
        }

        /// <summary>
        /// Compte le nombre de de sous-famille d'une famille
        /// </summary>
        /// <param name="FamilyId">id de la famille parent</param>
        public int CountSubFamilyOfFamily(int FamilyId)
        {
            var Result = 0;
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT count(*) FROM SousFamilles WHERE RefFamille = @refFamily", Connection))
                {
                    Command.Parameters.AddWithValue("@refFamily", FamilyId);
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

        /// <summary>
        /// Supprime la sous-famille dans la base
        /// Si la sous-famille est utilisé des articles alors un message d'erreur est renvoyé.
        /// </summary>
        /// <param name="Id">Id de la sous-famille à supprimer</param>
        public void Delete(int Id)
        {
            var UseCount = DaoRegistery.GetInstance.DaoArticle.CountArticleOfSubFamily(Id);

            // Vérifie si la sousfamille est utilisée par un article, et renvoie un message d'erreur si c'est le cas
            if (UseCount > 0)
            {
                string Error;
                if (UseCount == 1)
                {
                    Error = "Cette sous-famille est utilisée par 1 article, veuilliez supprimer l'article utilisant cette sous-famille avant de la supprimer.";
                }
                else
                {
                    Error = String.Format(
                        "Cette sous-famille est utilisée par {0} articles, veuilliez supprimer les articles utilisant cette sous-famille avant de la supprimer.",
                        UseCount);
                }
                throw new ArgumentException(Error);
            }
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("DELETE FROM SousFamilles WHERE RefSousFamille = @ref", Connection) )
                {
                    Command.Parameters.AddWithValue("@ref", Id);
                    Command.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Cherche les sous-familles d'une famille parent en utilisant le nom de la famille
        /// </summary>
        /// <param name="SubFamilyName">Id de la marque à supprimer</param>
        public List<SousFamilles> GetSubFamiliesByName(string SubFamilyName)
        {
            List<SousFamilles> SubFamilies;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM SousFamilles WHERE Nom = @name", Connection))
                {
                    Command.Parameters.AddWithValue("@name", SubFamilyName);
                    using (var Reader = Command.ExecuteReader())
                    {
                        SubFamilies = ParseQueryResult(Reader);
                    }
                }
            }

            return SubFamilies;
        }

        public List<SousFamilles> GetSubFamilyOfFamily(int FamilyId)
        {
            List<SousFamilles> SubFamilies;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM SousFamilles WHERE RefFamille = @refFamily", Connection))
                {
                    Command.Parameters.AddWithValue("@refFamily", FamilyId);
                    using (var Reader = Command.ExecuteReader())
                    {
                        SubFamilies = ParseQueryResult(Reader);
                    }
                }
            }

            return SubFamilies;
        }
    }
}
