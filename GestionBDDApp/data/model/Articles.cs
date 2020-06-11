namespace GestionBDDApp.data.model
{
    /// <summary>
    /// Représente un article en base
    /// </summary>
    public class Articles
    {
        /// <summary>
        /// Une référence unique sous la forme d'une taille max de 8 caractère
        /// </summary>
        public string RefArticle { get; }

        /// <summary>
        /// Une description de l'article
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// La Sous-Famille à laquelle appartient cet article
        /// </summary>
        public SousFamilles SousFamille { get; }

        /// <summary>
        /// La marque de cet article
        /// </summary>
        public Marques Marque { get; }

        /// <summary>
        /// Le prix de cet article
        /// </summary>
        public float Prix { get; }

        /// <summary>
        /// La quantité en stock de cet article
        /// </summary>
        public int Quantite { get; }

        /// <summary>
        /// Créer un article avec les valeurs passées en paramètres
        /// </summary>
        /// <param name="RefArticle"></param>
        /// <param name="Description"></param>
        /// <param name="SousFamille"></param>
        /// <param name="Marque"></param>
        /// <param name="Prix"></param>
        /// <param name="Quantite"></param>
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
