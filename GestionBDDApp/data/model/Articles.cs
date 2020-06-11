using System;

namespace GestionBDDApp.data.model
{
    public class Articles
    {
        public string RefArticle { get; set; }

        public string Description { get; set; }

        public SousFamilles SousFamille { get; set; }

        public Marques Marque { get; set; }

        public float Prix { get; set; }

        public int Quantite { get; set; }
        

        public Articles(string RefArticle, string Description, SousFamilles SousFamille, Marques Marque, float Prix,
            int Quantite)
        {
            this.RefArticle = RefArticle;
            this.Description = Description;
            this.SousFamille = SousFamille;
            this.Marque = Marque;
            this.Prix = Prix;
            this.Quantite = Quantite;
        }
    }
}
