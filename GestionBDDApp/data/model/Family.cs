namespace GestionBDDApp.data.model
{
    /// <summary>
    /// Représente un famille d'articles en base.
    /// </summary>
    public class Family
    {
        /// <summary>
        /// Un identifiant auto-incrémenté.
        /// s'il vaut null, la famille n'est pas encore sauvegardée en base.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Le nom de cette famille (entre 1 et 50 caractères).
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Crée une nouvelle famille.
        /// </summary>
        /// <param name="Id">L'id de la famille, null si cet objet n'est pas encore enregistré en base.</param>
        /// <param name="Name">Le nom de cette famille (entre 1 et 50 caractères).</param>
        public Family(int? Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
    }
}
