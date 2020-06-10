﻿using System;
 using GestionBDDApp.data.model;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GestionBDDApp.data.dao
{
    public class DAOSousFamille : AbstractDao
    {
        private readonly DAOFamille DaoFamille;

        public DAOSousFamille(DAOFamille DaoFamille) : base("SousFamilles")
        {
            this.DaoFamille = DaoFamille;
        }

        public List<SousFamilles> GetAllSousFamilles()
        {
            List<SousFamilles> SousFamilles;

            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM SousFamilles", Connection))
                {
                    using (var Reader = Command.ExecuteReader())
                    {
                        SousFamilles = ParseQueryResult(Reader);
                    }
                }
            }

            return SousFamilles;
        }

        private List<SousFamilles> ParseQueryResult(SQLiteDataReader DataReader)
        {
            List<SousFamilles> SousFamilles = new List<SousFamilles>();
            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    Familles Famille = this.DaoFamille.GetFamilleById(DataReader.GetInt32(1));
                    SousFamilles.Add(new SousFamilles(DataReader.GetInt32(0), Famille, DataReader.GetString(2)));
                }
            }

            return SousFamilles;
        }

        public SousFamilles getSousFamilleById(int id)
        {
            SousFamilles SousFamilles = null;

            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand( "SELECT * FROM SousFamilles WHERE RefSousFamille = @refSousFamille", Connection))
                { 
                    Command.Parameters.AddWithValue("@refSousFamille", id);
                    using (var Reader = Command.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            Reader.Read();
                            Familles Famille = this.DaoFamille.GetFamilleById(Reader.GetInt32(1));
                            SousFamilles = new SousFamilles(Reader.GetInt32(0), Famille, Reader.GetString(2));
                        }
                    }
                }
            }

            return SousFamilles;
        }

        public void save(SousFamilles SousFamille)
        {
            using (var Connection = new SQLiteConnection(ConnectionString))
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
        public int CountSubFamilyOfFamily(int FamilyId)
        {
            var Result = 0;
            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT count(*) FROM Familles WHERE RefFamille = @refFamily", Connection))
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

        public void delete(int Id)
        {
            var UseCount = DaoRegistery.GetInstance.DaoArticle.CountArticleOfSubFamily(Id);
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
            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("DELETE FROM SousFamilles WHERE RefSousFamille = @ref", Connection) )
                {
                    Command.Parameters.AddWithValue("@ref", Id);
                    Command.ExecuteNonQuery();
                }
            }
        }        
        
        public List<SousFamilles> GetSubFamiliesByName(string SubFamilyName)
        {
            List<SousFamilles> SubFamilies;

            using (var Connection = new SQLiteConnection(ConnectionString))
            {
                Connection.Open();
                using (var Command = new SQLiteCommand("SELECT * FROM SousFamilles WHERE Nom = '@name'", Connection))
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
    }
}
