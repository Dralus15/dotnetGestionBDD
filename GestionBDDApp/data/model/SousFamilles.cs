namespace GestionBDDApp.data.model
{
    /// <summary>
    /// Représente une sous-famille d'articles en base
    /// </summary>
    public class SousFamilles
    {
        /// <summary>
        /// Un identifiant auto-incrémenté;
        /// s'il est nul, l'article n'est pas encore sauvegardé en base
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// La famille à laquelle appartient cette sous-famille
        /// </summary>
        public Familles Famille { get; set; }

        /// <summary>
        /// Le nom de cette sous-famille (entre 5 et 11 caractères)
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Creer une nouvelle sous-famille
        /// </summary>
        /// <param name="Id">L'id de la sous-famille, null si cet object n'est pas encore enregistré en base</param>
        /// <param name="Famille">La famille à laquelle appartient cette sous-famille</param>
        /// <param name="Nom">Le nom de cette sous-famille (entre 5 et 11 caractères)</param>
        public SousFamilles(int? Id, Familles Famille, string Nom)
        {
            this.Id = Id;
            this.Famille = Famille;
            this.Nom = Nom;
        }
    }
}
