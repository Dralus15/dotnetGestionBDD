using System;

namespace GestionBDDApp.data.dto
{
    /// <summary>
    /// Représente une ligne dans le fichier d'import
    /// </summary>
    public class ArticlesDto
    {
        /// <summary>
        /// Référence unique de l'article (entre 3 et 8 caractères)
        /// </summary>
        public string ArticleRef { get; }

        /// <summary>
        /// Description de l'article (entre 3 et 150 caractères)
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Nom de la sous famille de l'article (entre 3 et 50 caractères)
        /// </summary>
        public string SubFamilyName { get; }

        /// <summary>
        /// Nom de la marque de l'article (entre 3 et 50 caractères)
        /// </summary>
        public string BrandName { get; }

        /// <summary>
        /// Prix de l'article
        /// </summary>
        public float Price { get; }

        /// <summary>
        /// Nom de la famille de l'article (entre 3 et 50 caractères)
        /// </summary>
        public string FamilyName { get; }

        /// <summary>
        /// Constructeur privé initialisant les champs de ce DTO
        /// </summary>
        /// <param name="ArticleRef">La référence de l'article</param>
        /// <param name="Description">La description de l'article</param>
        /// <param name="FamilyName">Le nom de la famille de l'article</param>
        /// <param name="SubFamilyName">Le nom de la sous famille de l'article</param>
        /// <param name="BrandName">Le nom de la marque de l'article</param>
        /// <param name="Price">Le prix de l'article</param>
        private ArticlesDto(string ArticleRef, string Description, string FamilyName, string SubFamilyName, 
            string BrandName, float Price)
        {
            this.ArticleRef = ArticleRef;
            this.Description = Description;
            this.SubFamilyName = SubFamilyName;
            this.BrandName = BrandName;
            this.Price = Price;
            this.FamilyName = FamilyName;
        }

        /// <summary>
        /// Créer un nouvel <code>ArticleDto</code> à partir de paramètre sous la forme de chaîne de caractères.
        /// C'est ici que les condition métiers et techniques sur les Articles sont testées (longueurs des champs).
        /// </summary>
        /// <param name="ArticleRef">La référence de l'article (entre 3 et 8 caractères)</param>
        /// <param name="Description">La description de l'article (entre 3 et 150 caractères)</param>
        /// <param name="FamilyName">Le nom de la famille de l'article (entre 3 et 50 caractères)</param>
        /// <param name="SubFamilyName">Le nom de la sous famille de l'article (entre 3 et 50 caractères)</param>
        /// <param name="BrandName">Le nom de la marque de l'article (entre 3 et 50 caractères)</param>
        /// <param name="PriceAsString">Le prix de l'article</param>
        /// <returns>Un nouvel <code>ArticleDto</code> initialisé</returns>
        /// <exception cref="ParsingException">Si le prix n'est pas correcte, ou que les paramètres ne satisfont pas les
        /// conditions métiers</exception>
        public static ArticlesDto FromRawData(string ArticleRef, string Description, string FamilyName, string SubFamilyName,
            string BrandName, string PriceAsString)
        {
            float ParsedPrice;

            if (Description == null || Description.Length > 150 || Description.Length < 3)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne description");
            }
            
            if (ArticleRef == null || ArticleRef.Length > 8 || ArticleRef.Length < 3)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne ref");
            }
            
            if (FamilyName == null || FamilyName.Length > 50 || FamilyName.Length < 3)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne famille");
            }
            
            if (SubFamilyName == null || SubFamilyName.Length > 50 || SubFamilyName.Length < 3)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne sous-famille");
            }
            
            if (BrandName == null || BrandName.Length > 50 || BrandName.Length < 3)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne marque");
            }
            
            try
            {
                ParsedPrice = float.Parse(PriceAsString);
            }
            catch (Exception)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne prix");
            }

            return new ArticlesDto(ArticleRef, Description, FamilyName, SubFamilyName, BrandName, ParsedPrice);
        }
    }
}