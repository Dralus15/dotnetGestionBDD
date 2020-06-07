using GestionBDDApp.data.model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionBDDApp.data
{
    class DAOFamille
    {
        private const string DbPath = "Data Source=Bacchus.SQLite";

        private SQLiteConnection connection;


        public void NewFamille(Familles Famille)
        {
            SQLiteCommand Requete = new SQLiteCommand("INSERT INTO Familles (RefFamille, Nom) VALUES (" + Famille.getId() + ", " + Famille.getNom() + ");", connection);
        }

        public List<Familles> GetAllFamilles()
        {
            List<Familles> Familles = new List<Familles>();

            SQLiteCommand Requete = new SQLiteCommand("SELECT * FROM Familles;",
                connection);
            SQLiteDataReader DataReader = Requete.ExecuteReader();

            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    Familles.Add(new Familles(DataReader.GetInt32(0), DataReader.GetString(1)));

                }
            }

            return Familles;
        }


        public Familles GetFamilleById(int Id)
        {
            Familles Famille;
            SQLiteCommand Requete = new SQLiteCommand(
                "SELECT * FROM Familles WHERE RefFamille = " + Id + ";",
                connection);
            SQLiteDataReader DataReader = Requete.ExecuteReader();

            if (DataReader.HasRows)
            {
                DataReader.Read();
                Famille = new Familles(DataReader.GetInt32(0), DataReader.GetString(1));
                return Famille;
            }
            else
            {
                return null;
            }
        }


        public Familles GetFamilleByNom(string Nom)
        {
            Familles Famille;
            SQLiteCommand Requete = new SQLiteCommand(
                "SELECT * FROM Familles WHERE Nom = " + Nom + ";",
                connection);
            SQLiteDataReader DataReader = Requete.ExecuteReader();

            if (DataReader.HasRows)
            {
                DataReader.Read();
                Famille = new Familles(DataReader.GetInt32(0), DataReader.GetString(1));
                return Famille;
            }
            else
            {
                return null;
            }
        }
    }
}
