namespace GestionBDDApp.data.model
{
    /// <summary>
    /// Représente un article en base.
    /// </summary>
    public class Articles
    {
        /// <summary>
        /// Une référence unique (entre 3 et 8 caractères).
        /// </summary>
        public string RefArticle { get; }

        /// <summary>
        /// Une description de l'article (entre 3 et 150 caractères).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// La Sous-Famille à laquelle appartient cet article.
        /// </summary>
        public SousFamilles SousFamille { get; set; }

        /// <summary>
        /// La marque de cet article.
        /// </summary>
        public Marques Marque { get; set; }

        /// <summary>
        /// Le prix de cet article.
        /// </summary>
        public float Prix { get; set; }

        /// <summary>
        /// La quantité en stock de cet article.
        /// </summary>
        public int Quantite { get; set; }

        /// <summary>
        /// Crée un article avec les valeurs passées en paramètres.
        /// </summary>
        /// <param name="RefArticle">Une référence unique (entre 3 et 8 caractères).</param>
        /// <param name="Description">Une description de l'article (entre 3 et 150 caractères).</param>
        /// <param name="SousFamille">Une sous-famille.</param>
        /// <param name="Marque">La marque de l'article.</param>
        /// <param name="Prix">Le prix de l'article (float >= 0).</param>
        /// <param name="Quantite">Le prix de l'article (float >= 0).</param>
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
