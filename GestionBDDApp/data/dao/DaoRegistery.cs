namespace GestionBDDApp.data.dao
{
    public class DaoRegistery
    {
        public DAOSousFamille DaoSousFamille { get; }
        public DAOArticle DaoArticle { get; }
        public DAOFamille DaoFamille { get; }
        public DAOMarque DaoMarque { get; }
        
        private static DaoRegistery Instance = null;

        private DaoRegistery()
        {
            DaoMarque = new DAOMarque();
            DaoFamille = new DAOFamille();
            DaoSousFamille = new DAOSousFamille(DaoFamille);
            DaoArticle = new DAOArticle(DaoSousFamille, DaoMarque);
        }

        public void ClearAll()
        {
            DaoArticle.clear();
            DaoSousFamille.clear();
            DaoMarque.clear();
            DaoFamille.clear();
        }

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