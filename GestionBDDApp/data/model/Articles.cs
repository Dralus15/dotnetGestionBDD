using System;

namespace GestionBDDApp.data.model
{
    public class Articles
    {
        public string RefArticle { get; }

        public string Description { get; }

        public SousFamilles SousFamille { get; }

        public Marques Marque { get; }

        public float Prix { get; }

        public int Quantite { get; }

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
