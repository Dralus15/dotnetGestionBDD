namespace GestionBDDApp.data.dao
{
    /// <summary>
    /// Point d'accès aux différents DAO, c'est un Singleton
    /// </summary>
    public class DaoRegistery
    {
        /// <summary>
        /// Le DAO des sous-familles
        /// </summary>
        public DaoSousFamille DaoSousFamille { get; }
        
        /// <summary>
        /// Le DAO des articles
        /// </summary>
        public DaoArticle DaoArticle { get; }
        
        /// <summary>
        /// Le DAO des familles
        /// </summary>
        public DaoFamille DaoFamille { get; }
        
        /// <summary>
        /// Le DAO des marques
        /// </summary>
        public DaoMarque DaoMarque { get; }
        
        /// <summary>
        /// Unique instance du registre (Singleton)
        /// </summary>
        private static DaoRegistery Instance;

        /// <summary>
        /// Constructeur privé du registre, instancie les DAOs
        /// </summary>
        private DaoRegistery()
        {
            DaoMarque = new DaoMarque();
            DaoFamille = new DaoFamille();
            DaoSousFamille = new DaoSousFamille(DaoFamille);
            DaoArticle = new DaoArticle(DaoSousFamille, DaoMarque);
        }

        /// <summary>
        /// Vide la base de donnée
        /// </summary>
        public void ClearAll()
        {
            DaoArticle.Clear();
            DaoSousFamille.Clear();
            DaoMarque.Clear();
            DaoFamille.Clear();
        }

        /// <summary>
        /// Accesseur du singleton
        /// </summary>
        public static DaoRegistery GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    Instance = new DaoRegistery();
                }
                return Instance;
            }
        }
        
    }
}