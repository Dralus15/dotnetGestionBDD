using System;

namespace GestionBDDApp.data.dto
{
    public class ArticlesDto
    {
        public string RefArticle { get; }

        public string Description { get; }

        public string SousFamille { get; }

        public string Marque { get; }

        public float Prix { get; }

        public string Famille { get; }

        private ArticlesDto(string RefArticle, string Description, string SousFamille, string Marque, float Prix, string Famille)
        {
            this.RefArticle = RefArticle;
            this.Description = Description;
            this.SousFamille = SousFamille;
            this.Marque = Marque;
            this.Prix = Prix;
            this.Famille = Famille;
        }
        
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
            catch (Exception)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne prix");
            }

            return new ArticlesDto(RefStr, DescStr, SousFamilleStr, MarqueStr, Prix, FamilleStr);
        }
    }
}