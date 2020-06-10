
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
            List<Articles> Articles = new List<Articles>();

            SQLiteCommand Requete = new SQLiteCommand("SELECT * FROM Articles;", NewConnection());
            SQLiteDataReader DataReader = Requete.ExecuteReader();

            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    SousFamilles SousFamille = DaoSousFamille.getSousFamilleById(DataReader.GetInt32(2));
                    Marques Marque = DaoMarque.GetMarqueById(DataReader.GetInt32(3));
                    
                    Articles.Add(new Articles(DataReader.GetString(0), DataReader.GetString(1), SousFamille, Marque, DataReader.GetFloat(4), DataReader.GetInt32(5)));
                }
                DataReader.Close();
            }

            return Articles;
        }

        public void save(Articles Article)
        {
            var Command = new SQLiteCommand(NewConnection());
            Command.CommandText = @"INSERT INTO Articles(RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@ref,@desc, @refSousFamille, @refMarque, @prix, @quantity) " + 
                                      "ON CONFLICT(RefArticle) DO UPDATE SET Description=@desc, RefSousFamille=@refSousFamille, RefMarque=@refMarque, PrixHT=@prix, Quantite=@quantity";
            Command.Parameters.AddWithValue("@ref", Article.RefArticle);
            Command.Parameters.AddWithValue("@desc", Article.Description);
            Command.Parameters.AddWithValue("@refSousFamille", Article.SousFamille.Id);
            Command.Parameters.AddWithValue("@refMarque", Article.Marque.Id);
            Command.Parameters.AddWithValue("@prix", Article.Prix);
            Command.Parameters.AddWithValue("@quantity", Article.Quantite);
            Command.ExecuteNonQueryAsync();
        }
    }
}
