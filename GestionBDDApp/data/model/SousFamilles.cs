using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionBDDApp.data.model
{
    class SousFamilles
    {
        //auto-increment
        private int Id;

        private Familles Famille;

        private string Nom;


        public SousFamilles(int Id, Familles Famille, string Nom)
        {
            this.Id = Id;
            this.Famille = Famille;
            this.Nom = Nom;
        }


        public SousFamilles(int Id, int IdFamille, string NomFamille, string Nom)
        {
            Familles Famille = new Familles(IdFamille, NomFamille);
            this.Id = Id;
            this.Famille = Famille;
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


        public Familles getFamille()
        {
            return this.Famille;
        }


        public void setFamille(Familles Famille)
        {
            this.Famille = Famille;
        }
    }
}
