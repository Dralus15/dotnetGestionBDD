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
        public int? Id { get; set; }

        public string Nom { get; }

        public Marques(int? Id, string Nom)
        {
            this.Nom = Nom;
            this.Id = Id;
        }
    }
}
