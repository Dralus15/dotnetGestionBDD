namespace GestionBDDApp.data.model
{
    /// <summary>
    /// Représente une marque d'article en base.
    /// </summary>
    public class Marques
    {
        /// <summary>
        /// Un identifiant auto-incrémenté.
        /// s'il vaut null, la marque n'est pas encore sauvegardée en base.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Le nom de cette marque (entre 3 et 50 caractères).
        /// </summary>
        public string Nom { get; set; }
        
        /// <summary>
        /// Crée une nouvelle marque.
        /// </summary>
        /// <param name="Id">L'id de la marque, null si cet objet n'est pas encore enregistré en base.</param>
        /// <param name="Nom">Le nom de cette marque (entre 3 et 50 caractères).</param>
        public Marques(int? Id, string Nom)
        {
            this.Nom = Nom;
            this.Id = Id;
        }
    }
}
