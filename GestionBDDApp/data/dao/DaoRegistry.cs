namespace GestionBDDApp.data.dao
{
    /// <summary>
    /// Point d'accès aux différents DAO, c'est un Singleton.
    /// </summary>
    public class DaoRegistry
    {
        /// <summary>
        /// Le DAO des sous-familles.
        /// </summary>
        public SubFamilyDao SubFamilyDao { get; }
        
        /// <summary>
        /// Le DAO des articles.
        /// </summary>
        public ArticleDao ArticleDao { get; }
        
        /// <summary>
        /// Le DAO des familles.
        /// </summary>
        public FamilyDao FamilyDao { get; }
        
        /// <summary>
        /// Le DAO des marques.
        /// </summary>
        public BrandDao BrandDao { get; }
        
        /// <summary>
        /// Unique instance du registre (Singleton).
        /// </summary>
        private static DaoRegistry Instance;

        /// <summary>
        /// Constructeur privé du registre, instancie les DAOs.
        /// </summary>
        private DaoRegistry()
        {
            BrandDao = new BrandDao();
            FamilyDao = new FamilyDao();
            SubFamilyDao = new SubFamilyDao(FamilyDao);
            ArticleDao = new ArticleDao(SubFamilyDao, BrandDao);
        }

        /// <summary>
        /// Vide la base de données.
        /// </summary>
        public void ClearAll()
        {
            ArticleDao.Clear();
            SubFamilyDao.Clear();
            BrandDao.Clear();
            FamilyDao.Clear();
        }

        /// <summary>
        /// Accesseur du singleton.
        /// </summary>
        public static DaoRegistry GetInstance => Instance ?? (Instance = new DaoRegistry());
    }
}