namespace GestionBDDApp.data.dao
{
    public class DaoRegistery
    {
        public DaoSousFamille DaoSousFamille { get; }
        public DaoArticle DaoArticle { get; }
        public DaoFamille DaoFamille { get; }
        public DaoMarque DaoMarque { get; }
        
        private static DaoRegistery _instance;

        private DaoRegistery()
        {
            DaoMarque = new DaoMarque();
            DaoFamille = new DaoFamille();
            DaoSousFamille = new DaoSousFamille(DaoFamille);
            DaoArticle = new DaoArticle(DaoSousFamille, DaoMarque);
        }

        public void ClearAll()
        {
            //TODO metre les index d'auto increment a 0
            DaoArticle.Clear();
            DaoSousFamille.Clear();
            DaoMarque.Clear();
            DaoFamille.Clear();
        }

        public static DaoRegistery GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DaoRegistery();
                }
                return _instance;
            }
        }
        
    }
}