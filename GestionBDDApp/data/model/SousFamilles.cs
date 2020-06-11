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
        public Familles Famille { get; }

        /// <summary>
        /// Le nom de cette sous-famille
        /// </summary>
        public string Nom { get; }


        public SousFamilles(int? Id, Familles Famille, string Nom)
        {
            this.Id = Id;
            this.Famille = Famille;
            this.Nom = Nom;
        }
    }
}
