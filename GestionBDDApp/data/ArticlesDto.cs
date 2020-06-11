using System;

namespace GestionBDDApp.data.dto
{
    /// <summary>
    /// Représente une ligne dans le fichier d'import
    /// </summary>
    public class ArticlesDto
    {
        public string ArticleRef { get; }

        public string Description { get; }

        public string SubFamilyName { get; }

        public string BrandName { get; }

        public float Price { get; }

        public string FamilyName { get; }

        private ArticlesDto(string ArticleRef, string Description, string SubFamilyName, string BrandName, float Price, 
            string FamilyName)
        {
            this.ArticleRef = ArticleRef;
            this.Description = Description;
            this.SubFamilyName = SubFamilyName;
            this.BrandName = BrandName;
            this.Price = Price;
            this.FamilyName = FamilyName;
        }
        
        public static ArticlesDto FromRawData(string Description, string ArticleRef, string BrandName, string FamilyName,
            string SubFamily, string PriceAsString)
        {
            float ParsedPrice;

            if (Description == null || Description.Length > 150 || Description.Length < 5)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne description");
            }
            
            if (ArticleRef == null || ArticleRef.Length > 8 || ArticleRef.Length < 5)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne ref");
            }
            
            try
            {
                ParsedPrice = float.Parse(PriceAsString);
            }
            catch (Exception)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne prix");
            }

            return new ArticlesDto(ArticleRef, Description, SubFamily, BrandName, ParsedPrice, FamilyName);
        }
    }
}