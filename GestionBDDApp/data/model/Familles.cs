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
        public int? Id { get; set; }

        public string Nom { get; }
        
        public Familles(int? Id, string Nom)
        {
            this.Id = Id;
            this.Nom = Nom;
        }
    }
}
