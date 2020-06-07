using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionBDDApp.data.model
{
    public class Marques
    {
        //auto-increment
        private int Id;

        private string Nom;     

        public Marques(int Id, string Nom)
        {
            this.Nom = Nom;
            this.Id = Id;
        }


        public int getId()
        {
            return this.Id;
        }


        public void setId(int Id)
        {
            this.Id = Id;
        }


        public string getNom()
        {
            return (this.Nom);
        }


        public void setNom(string Nom)
        {
            this.Nom = Nom;
        }
    }
}
