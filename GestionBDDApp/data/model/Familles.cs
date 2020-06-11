namespace GestionBDDApp.data.model
{
    /// <summary>
    /// Représente un famille d'articles en base.
    /// </summary>
    public class Familles
    {
        /// <summary>
        /// Un identifiant auto-incrémenté.
        /// s'il vaut null, la famille n'est pas encore sauvegardée en base.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Le nom de cette famille (entre 3 et 50 caractères).
        /// </summary>
        public string Nom { get; set; }
        
        /// <summary>
        /// Crée une nouvelle famille.
        /// </summary>
        /// <param name="Id">L'id de la famille, null si cet objet n'est pas encore enregistré en base.</param>
        /// <param name="Nom">Le nom de cette famille (entre 3 et 50 caractères).</param>
        public Familles(int? Id, string Nom)
        {
            this.Id = Id;
            this.Nom = Nom;
        }
    }
}
