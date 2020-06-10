﻿﻿
using System.Collections.Generic;
using System.Data.SQLite;
using GestionBDDApp.data.model;

namespace GestionBDDApp.data.dao
{
    public class DAOArticle : AbstractDao
    {
        private readonly DAOSousFamille DaoSousFamille;
        private readonly DAOMarque DaoMarque;

        public DAOArticle(DAOSousFamille DaoSousFamille, DAOMarque DaoMarque): base("Articles")
        {
            this.DaoSousFamille = DaoSousFamille;
            this.DaoMarque = DaoMarque;
        }

        public List<Articles> getAll()
        {
            var Articles = new List<Articles>();

            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM Articles;", Connection))
                {
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (!Reader.HasRows) return Articles;
                        while (Reader.Read())
                        {
                            var SousFamille = DaoSousFamille.getSousFamilleById(Reader.GetInt32(2));
                            var Marque = DaoMarque.GetMarqueById(Reader.GetInt32(3));
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

        public Articles GetArticleById(string Id)
        {
            Articles Article = null;

            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM Articles WHERE RefArticle = @refArticle", Connection))
                {
                    Command.Parameters.AddWithValue("@refArticle", Id);
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            var SousFamille = DaoSousFamille.getSousFamilleById(Reader.GetInt32(2));
                            var Marque = DaoMarque.GetMarqueById(Reader.GetInt32(3));
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

        public void save(Articles Article)
        {
            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand(
                    @"INSERT INTO Articles(RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@ref,@desc, @refSousFamille, @refMarque, @prix, @quantity)"
                    , Connection)
                )
                {
                    Command.Parameters.AddWithValue("@ref", Article.RefArticle);
                    Command.Parameters.AddWithValue("@desc", Article.Description);
                    Command.Parameters.AddWithValue("@refSousFamille", Article.SousFamille.Id);
                    Command.Parameters.AddWithValue("@refMarque", Article.Marque.Id);
                    Command.Parameters.AddWithValue("@prix", Article.Prix);
                    Command.Parameters.AddWithValue("@quantity", Article.Quantite);
                    Command.ExecuteNonQuery();
                }
            }
        }

        public void delete(string Id)
        {
            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("DELETE FROM Articles WHERE RefArticle = @ref", Connection) )
                {
                    Command.Parameters.AddWithValue("@ref", Id);
                    Command.ExecuteNonQuery();
                }
            }
        }
    }
}
