﻿
using System.Collections.Generic;
using System.Data.SQLite;
using GestionBDDApp.data.model;

namespace GestionBDDApp.data.dao
{
    /// <summary>
    /// Classe du Dao pour les articles, elle permet de faire des traitements sur la table Articles
    /// </summary>
    public class ArticleDao : AbstractDao
    {
        /// <summary>
        /// Dao des sous-familles pour la référence des familles/sous-familles dans l'article
        /// </summary>
        private readonly SubFamilyDao SubFamilyDao;
        /// <summary>
        /// Dao des marques pour la référence des marques dans l'article
        /// </summary>
        private readonly BrandDao BrandDao;

        /// <summary>
        /// Instancie le Dao des articles
        /// </summary>
        /// <param name="SubFamilyDao">Dao de la sous-famille</param>
        /// <param name="BrandDao">Dao de la marque</param>
        public ArticleDao(SubFamilyDao SubFamilyDao, BrandDao BrandDao)
            : base("Articles", false)
        {
            this.SubFamilyDao = SubFamilyDao;
            this.BrandDao = BrandDao;
        }

        /// <summary>
        /// Récupère tout les articles dans la base et les retourne dans une <b>List'&lt;'Articles'&gt;'</b>
        /// </summary>
        public List<Articles> GetAll()
        {
            var Articles = new List<Articles>();

            // On se connecte à la base de donnée pour envoyer la requête
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();

                // Création de la requête
                using (var Command = new SQLiteCommand("SELECT * FROM Articles;", Connection))
                {
                    // On récupère la requête et on la parse pour obtenir une liste des articles 
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (!Reader.HasRows) return Articles;
                        while (Reader.Read())
                        {
                            var SousFamille = SubFamilyDao.GetSousFamilleById(Reader.GetInt32(2));
                            var Marque = BrandDao.GetMarqueById(Reader.GetInt32(3));
                            if (Marque == null || SousFamille == null)
                            {
                                continue;
                            }
                            Articles.Add(new Articles(
                                Reader.GetString(0), 
                                Reader.GetString(1), 
                                SousFamille, Marque, 
                                Reader.GetFloat(4), 
                                Reader.GetInt32(5)));
                        }
                    }
                }
            }

            return Articles;
        }

        /// <summary>
        /// Compte le nombre de d'article d'une sous-famille
        /// </summary>
        /// <param name="SubFamilyId">id de la sous-famille</param>
        public int CountArticleOfSubFamily(int SubFamilyId)
        {
            var Result = 0;
            
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "SELECT count(*) FROM Articles WHERE RefSousFamille = @refSubFamily", Connection))
                {
                    Command.Parameters.AddWithValue("@refSubFamily", SubFamilyId);
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
        /// Compte le nombre de d'article d'une marque
        /// </summary>
        /// <param name="BrandId">id de la marque</param>
        public int CountArticleOfBrand(int BrandId)
        {
            var Result = 0;
            
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "SELECT count(*) FROM Articles WHERE RefMarque = @refBrand", Connection))
                {
                    Command.Parameters.AddWithValue("@refBrand", BrandId);
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
        /// Cherche l'article par id et le retourne
        /// </summary>
        /// <param name="Id">id de l'article recherché</param>
        public Articles GetArticleById(string Id)
        {
            Articles Article = null;

            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "SELECT * FROM Articles WHERE RefArticle = @refArticle", Connection))
                {
                    Command.Parameters.AddWithValue("@refArticle", Id);
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            var SousFamille = SubFamilyDao.GetSousFamilleById(Reader.GetInt32(2));
                            var Marque = BrandDao.GetMarqueById(Reader.GetInt32(3));
                            Article = new Articles(
                                Reader.GetString(0), 
                                Reader.GetString(1), 
                                SousFamille, Marque, 
                                Reader.GetFloat(4), 
                                Reader.GetInt32(5));
                        }
                    }
                }
            }

            return Article;
        }

        /// <summary>
        /// Crée un article dans la base
        /// </summary>
        /// <param name="Article">article à créer</param>
        public void Create(Articles Article)
        {
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    @"INSERT INTO Articles(RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES" 
                    + @" (@ref,@desc, @refSousFamille, @refMarque, @prix, @quantity)"
                    , Connection)
                )
                {
                    Command.Parameters.AddWithValue("@ref", Article.RefArticle);
                    Command.Parameters.AddWithValue("@desc", Article.Description);
                    Command.Parameters.AddWithValue("@refSousFamille", Article.SubFamily.Id);
                    Command.Parameters.AddWithValue("@refMarque", Article.Marque.Id);
                    Command.Parameters.AddWithValue("@prix", Article.Price);
                    Command.Parameters.AddWithValue("@quantity", Article.Quantity);
                    Command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Modifie un article dans la base
        /// </summary>
        /// <param name="Article">article à modifier</param>
        public void Update(Articles Article)
        {
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    "UPDATE Articles SET Description = @desc, RefSousFamille = @refSousFamille, RefMarque = @refMarque" 
                    + ", PrixHT = @prix, Quantite = @quantity WHERE RefArticle = @ref"
                    , Connection)
                )
                {
                    Command.Parameters.AddWithValue("@ref", Article.RefArticle);
                    Command.Parameters.AddWithValue("@desc", Article.Description);
                    Command.Parameters.AddWithValue("@refSousFamille", Article.SubFamily.Id);
                    Command.Parameters.AddWithValue("@refMarque", Article.Marque.Id);
                    Command.Parameters.AddWithValue("@prix", Article.Price);
                    Command.Parameters.AddWithValue("@quantity", Article.Quantity);
                    Command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Supprime un article dans la base.
        /// </summary>
        /// <param name="ArticleRef">Référence de l'article à supprimer</param>
        public void Delete(string ArticleRef)
        {
            using (var Connection = new SQLiteConnection(CONNECTION_STRING))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("DELETE FROM Articles WHERE RefArticle = @ref", Connection))
                {
                    Command.Parameters.AddWithValue("@ref", ArticleRef);
                    Command.ExecuteNonQuery();
                }
            }
        }

    }
}
