using GestionBDDApp.data.model;
using GestionBDDApp.data.dao;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionBDDApp.data.dao
{
    class DAOSousFamille
    {
        private const string DbPath = "Data Source=Bacchus.SQLite";

        private SQLiteConnection connection;


        public void NewFamille(SousFamilles SousFamille)
        {
            SQLiteCommand Requete = new SQLiteCommand(
                "INSERT INTO SousFamilles (RefSousFamille, RefFamille, Nom) " +
                "VALUES (" + SousFamille.getId() + ", " + SousFamille.getFamille().getId() 
                + ", " + SousFamille.getNom() + ");", connection);
        }

        public List<SousFamilles> GetAllSousFamilles()
        {
            List<SousFamilles> SousFamilles = new List<SousFamilles>();

            SQLiteCommand Requete = new SQLiteCommand("SELECT * FROM SousFamilles",
                connection);
            SQLiteDataReader DataReader = Requete.ExecuteReader();

            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    DAOFamille DaoFamille = new DAOFamille();
                    Familles Famille = DaoFamille.GetFamilleById(DataReader.GetInt32(1));
                    SousFamilles.Add(new SousFamilles(DataReader.GetInt32(0),Famille, DataReader.GetString(2)));

                }
            }

            return SousFamilles;
        }

    }
}
