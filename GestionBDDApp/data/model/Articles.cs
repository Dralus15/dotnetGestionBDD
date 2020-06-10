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
        
        public Articles(string RefArticle, string Description, SousFamilles SousFamille, Marques Marque, float Prix, int Quantite)
        {
            this.RefArticle = RefArticle;
            this.Description = Description;
            this.SousFamille = SousFamille;
            this.Marque = Marque;
            this.Prix = Prix;
            this.Quantite = Quantite;
        }

        //TODO changer ca poru que ca corrige juste les champs
        public static ArticlesDto FromRawData(string DescStr, string RefStr, string MarqueStr, string FamilleStr,
            string SousFamilleStr, string PrixStr)
        {
            float Prix;

            if (DescStr == null || DescStr.Length > 150 || DescStr.Length < 5)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne description");
            }
            
            if (RefStr == null || RefStr.Length > 8 || RefStr.Length < 5)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne ref");
            }
            
            try
            {
                Prix = float.Parse(PrixStr);
            }
            catch (Exception Exception)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne prix");
            }

            return new ArticlesDto(RefStr, DescStr, SousFamilleStr, MarqueStr, Prix, FamilleStr);
        }

    }

    public class ArticlesDto
    {
        public string RefArticle { get; }

        public string Description { get; }

        public string SousFamille { get; }

        public string Marque { get; }

        public float Prix { get; }

        public string Famille { get; }

        public ArticlesDto(string RefArticle, string Description, string SousFamille, string Marque, float Prix, string Famille)
        {
            this.RefArticle = RefArticle;
            this.Description = Description;
            this.SousFamille = SousFamille;
            this.Marque = Marque;
            this.Prix = Prix;
            this.Famille = Famille;
        }
    }
}
