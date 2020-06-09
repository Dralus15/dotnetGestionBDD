using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionBDDApp.data.model
{
    public class SousFamilles
    {
        //auto-increment
        public int? Id { get; set; }

        public Familles Famille { get; }

        public string Nom { get; }


        public SousFamilles(int? Id, Familles Famille, string Nom)
        {
            this.Id = Id;
            this.Famille = Famille;
            this.Nom = Nom;
        }
    }
}
