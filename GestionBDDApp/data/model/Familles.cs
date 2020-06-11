namespace GestionBDDApp.data.model
{
    /// <summary>
    /// Représente un famille d'article en base
    /// </summary>
    public class Familles
    {
        /// <summary>
        /// Un identifiant auto-incrémenté;
        /// s'il est nul, l'article n'est pas encore sauvegardé en base
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Le nom de cette famille
        /// </summary>
        public string Nom { get; set; }
        
        /// <summary>
        /// Creer une nouvelle famille
        /// </summary>
        /// <param name="Id">L'id de la famille, null si cet object n'est pas encore enregistré en base</param>
        /// <param name="Nom">Le nom de cette famille (entre 5 et 50 caractères)</param>
        public Familles(int? Id, string Nom)
        {
            this.Id = Id;
            this.Nom = Nom;
        }
    }
}
