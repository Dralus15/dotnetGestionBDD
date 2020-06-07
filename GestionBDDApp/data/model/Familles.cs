using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionBDDApp.data.model
{
    public class Familles
    {
        //auto-increment
        private int Id;

        private string Nom;
    

        public Familles(int Id, string Nom)
        {
            this.Id = Id;
            this.Nom = Nom;
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
