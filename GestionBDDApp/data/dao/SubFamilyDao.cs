﻿using System;
 using GestionBDDApp.data.model;
using System.Collections.Generic;
 using System.Data.Common;
 using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    /// <summary>
    /// Classe du Dao pour les sous-familles, elle permet de faire des traitements sur la table SousFamilles.
    /// </summary>
    public class SubFamilyDao : AbstractDao
    {
        /// <summary>
        /// Le Dao de la famille parent des sous-familles.
        /// </summary>
        private readonly FamilyDao FamilyDao;

        /// <summary>
        /// Instancie le Dao des sous-familles, il faut le Dao de la famille parent.
        /// </summary>
        /// <param name="FamilyDao">Dao de la famille.</param>
        public SubFamilyDao(FamilyDao FamilyDao) : base("SousFamilles", true)
        {
            this.FamilyDao = FamilyDao;
        }

        /// <summary>
        /// Récupère toutes les sous-familles dans la base et les retourne dans une <b>List'&lt;'SousFamilles'&gt;'</b>
        /// </summary>
        public List<SubFamily> GetAllSousFamilles()
        {
            List<SubFamily> SousFamilles;

            // On se connecte à la base de données pour envoyer la requête.
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();

                // Création de la requête.
                using (var Command = new SQLiteCommand("SELECT * FROM SousFamilles", Connection))
                {
                    // On récupère la requête et on la parse pour obtenir une liste des sous-familles.
                    using (var Reader = Command.ExecuteReader())
                    {
                        SousFamilles = ParseQueryResult(Reader);
                    }
                }
            }

            return SousFamilles;
        }

        /// <summary>
        /// Parse le resultat de la requête SQL pour le retourner dans une <b>List'&lt;'SousFamilles'&gt;'</b>.
        /// </summary>
        /// <param name="DataReader">Le résultat de la requête SQL.</param>
        private List<SubFamily> ParseQueryResult(DbDataReader DataReader)
        {
            var SubFamilyRed = new List<SubFamily>();
            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    var Family = FamilyDao.GetFamilleById(DataReader.GetInt32(1));
                    SubFamilyRed.Add(new SubFamily(DataReader.GetInt32(0), Family, 
                        DataReader.GetString(2)));
                }
            }

            return SubFamilyRed;
        }

        /// <summary>
        /// Cherche une sous-famille par son id et la retourne.
        /// </summary>
        /// <param name="Id">id de la sous-famille recherchée.</param>
        public SubFamily GetSousFamilleById(int Id)
        {
            SubFamily SubFamilyFound = null;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "SELECT * FROM SousFamilles WHERE RefSousFamille = @refSousFamille", Connection))
                { 
                    Command.Parameters.AddWithValue("@refSousFamille", Id);
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            var Famille = FamilyDao.GetFamilleById(Reader.GetInt32(1));
                            SubFamilyFound = new SubFamily(Reader.GetInt32(0), Famille, 
                                Reader.GetString(2));
                        }
                    }
                }
            }

            return SubFamilyFound;
        }

        /// <summary>
        /// Sauvegarde la sous-famille dans la base (si elle existe déjà, alors elle est mise à jour).
        /// </summary>
        /// <param name="SubFamily">Sous-famille à sauvegarder.</param>
        public void Save(SubFamily SubFamily)
        {
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(Connection))
                {
                    if (SubFamily.Id == null)
                    {
                        Command.CommandText = "INSERT INTO SousFamilles(RefFamille, Nom) VALUES (@refFamille, @name)";
                    }
                    else
                    {
                        Command.CommandText = "UPDATE SousFamilles SET RefFamille=@refFamille, Nom=@name WHERE " + 
                                              "RefSousFamille = @refSousFamille";
                        Command.Parameters.AddWithValue("@refSousFamille", SubFamily.Id);
                    }
        
                    Command.Parameters.AddWithValue("@refFamille", SubFamily.Family.Id);
                    Command.Parameters.AddWithValue("@name", SubFamily.Name);
                    Command.ExecuteNonQuery();
                    if (SubFamily.Id == null)
                    {
                        SubFamily.Id = (int) Connection.LastInsertRowId;
                    }
                }
            }
        }

        /// <summary>
        /// Compte le nombre de de sous-familles d'une famille.
        /// </summary>
        /// <param name="FamilyId">id de la famille parent.</param>
        public int CountSubFamilyOfFamily(int FamilyId)
        {
            var Result = 0;
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "SELECT count(*) FROM SousFamilles WHERE RefFamille = @refFamily", Connection))
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
        /// Supprime la sous-famille dans la base.
        /// Si la sous-famille est utilisée par des articles alors un message d'erreur est renvoyé.
        /// </summary>
        /// <param name="Id">Id de la sous-famille à supprimer.</param>
        public void Delete(int Id)
        {
            var UseCount = DaoRegistry.GetInstance.ArticleDao.CountArticleOfSubFamily(Id);

            // Vérifie si la sousfamille est utilisée par un article, et renvoie un message d'erreur si c'est le cas.
            if (UseCount > 0)
            {
                string Error;
                if (UseCount == 1)
                {
                    Error = "Cette sous-famille est utilisée par 1 article, veuilliez supprimer l'article utilisant" 
                            + " cette sous-famille avant de la supprimer.";
                }
                else
                {
                    Error =
                        $"Cette sous-famille est utilisée par {UseCount} articles, veuilliez supprimer les articles " 
                        + "utilisant cette sous-famille avant de la supprimer.";
                }
                throw new ArgumentException(Error);
            }
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "DELETE FROM SousFamilles WHERE RefSousFamille = @ref", Connection))
                {
                    Command.Parameters.AddWithValue("@ref", Id);
                    Command.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Cherche des sous-familles avec un nom qui est celui passé en paramètre.
        /// Retourne une liste des sous familles trouvées.
        /// </summary>
        /// <param name="SubFamilyName">Nom de la sous-famille cherchée.</param>
        public List<SubFamily> GetSubFamiliesByName(string SubFamilyName)
        {
            List<SubFamily> SubFamilies;

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

        /// <summary>
        /// Cherche des sous-familles aillant comme famille parent la famille passée en paramètre.
        /// Retourne une liste des sous familles trouvées.
        /// </summary>
        /// <param name="FamilyId">Id de la famille parent.</param>
        public List<SubFamily> GetSubFamilyOfFamily(int FamilyId)
        {
            List<SubFamily> SubFamilies;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "SELECT * FROM SousFamilles WHERE RefFamille = @refFamily", Connection))
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
