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
        /// Le nom de cette marque
        /// </summary>
        public string Nom { get; set; }

        public Marques(int? Id, string Nom)
        {
            this.Nom = Nom;
            this.Id = Id;
        }
    }
}
