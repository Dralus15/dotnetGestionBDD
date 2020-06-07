using GestionBDDApp.data.model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionBDDApp.data
{
    class DAOMarque
    {
        private const string DbPath = "Data Source=Bacchus.SQLite";

        private SQLiteConnection connection;


        public void NewMarque(Marques Marque)
        {
            SQLiteCommand Requete = new SQLiteCommand("INSERT INTO Marques (RefMarque, Nom) VALUES (" + Marque.getId() + ", " + Marque.getNom() + ");", connection);
        }

        public List<Marques> GetAllMarques()
        {
            List<Marques> Marques = new List<Marques>();

            SQLiteCommand Requete = new SQLiteCommand( "SELECT * FROM Marques", 
                connection);
            SQLiteDataReader DataReader = Requete.ExecuteReader();

            if (DataReader.HasRows)
            {
                while (DataReader.Read())
                {
                    Marques.Add(new Marques(DataReader.GetInt32(0), 
                        DataReader.GetString(1)));

                }
            }

            return Marques;
        }
    }
}
