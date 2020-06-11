namespace GestionBDDApp.data.model
{
    /// <summary>
    /// Représente une marque d'article en base
    /// </summary>
    public class Marques
    {
        /// <summary>
        /// Un identifiant auto-incrémenté;
        /// s'il est nul, la marque n'est pas encore sauvegardée en base
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Le nom de cette marque (entre 5 et 11 caractères)
        /// </summary>
        public string Nom { get; set; }
        
        /// <summary>
        /// Creer une nouvelle marque
        /// </summary>
        /// <param name="Id">L'id de la marque, null si cet object n'est pas encore enregistré en base</param>
        /// <param name="Nom">Le nom de cette marque (entre 5 et 11 caractères)</param>
        public Marques(int? Id, string Nom)
        {
            this.Nom = Nom;
            this.Id = Id;
        }
    }
}
