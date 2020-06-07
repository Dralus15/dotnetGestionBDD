using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionBDDApp.data.model;

namespace GestionBDDApp.data
{
    class DAOArticle
    {
        private const string DbPath = "Data Source=Bacchus.SQLite";

        private SQLiteConnection connection;


        public void NewArticle(Articles Article)
        {
            /*SQLiteCommand Requete = new SQLiteCommand(
                "INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) " +
                "VALUES (" + Article.getId() + ", " + Famille.getNom() + ");", connection);*/
        }

    }
}
