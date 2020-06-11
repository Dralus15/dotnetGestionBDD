using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using GestionBDDApp.Properties;

namespace GestionBDDApp
{    
    /// <summary>
    /// Fenêtre principale de l'application.
    /// </summary>
    public partial class FormMain : Form
    {
        /// <summary>
        /// Noeud racine des articles
        /// </summary>
        private TreeNode AllArticles;

        /// <summary>
        /// Noeud racine des familles
        /// </summary>
        private TreeNode AllFamilyNode;

        /// <summary>
        /// Noeud racine des marques
        /// </summary>
        private TreeNode AllBrandNode;

        /// <summary>
        /// Dernier noeud séléctionné par l'utilisateur
        /// </summary>
        private TreeNode LastTreeNodeSelected;

        private List<Articles> ArticlesModel = new List<Articles>();

        private Dictionary<int?, List<SousFamilles>> SubFamilyModel = new Dictionary<int?, List<SousFamilles>>();

        private List<Familles> FamilyModel = new List<Familles>();

        private List<Marques> BrandModel = new List<Marques>();

        /// <summary>
        /// Un filtre potentiellement nul définissant l'id de sous-famille que doivent avoir les articles pour
        /// être affiché
        /// </summary>
        private int? SubFamilyFilter;
        
        /// <summary>
        /// Un filtre potentiellement nul définissant l'id de la marque que doivent avoir les articles pour
        /// être affiché
        /// </summary>
        private int? BrandFilter;

        /// <summary>
        /// Le type de donnée affiché dans la partie droite de l'écran
        /// </summary>
        private ActiveList ArticleViewOn = ActiveList.Unknown;

        /// <summary>
        /// La dernière colonne à avoir été trié
        /// </summary>
        private ColumnHeader LastSortedColumn;

        /// <summary>
        /// Index de la colonne contenant la quantité d'article
        /// (on la garde car c'est la seul qui n'est pas triable)
        /// </summary>
        private const int COLUMN_QUANTITY = 4;
        
        /// <summary>
        /// Créer la fênetre principale de l'application.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Charge la liste correspondant au noeud passé en paramètre
        /// </summary>
        /// <param name="TreeNodeSelected"></param>
        private void LoadCorrespondingList(TreeNode TreeNodeSelected)
        {
            listView1.BeginUpdate();
            BrandFilter = null;
            SubFamilyFilter = null;
            // si la liste est identique à la dernière, on ne change pas les colonnes
            if (LastTreeNodeSelected != TreeNodeSelected)
            {
                SupprColonne();
            }
            LastTreeNodeSelected = TreeNodeSelected;
            // on reset la liste
            listView1.Groups.Clear();
            listView1.SelectedItems.Clear();
            listView1.Items.Clear();
            
            // Si aucun noeud n'est séléctioné on ne fait rien
            if (LastTreeNodeSelected == null) { }
            // Si le noeud des articles est séléctioné
            if (TreeNodeSelected.Equals(AllArticles))
            {
                // on charge les articles
                LoadArticles();
                // on les affiches
                DisplayArticlesWithFilter();
            }
            else
            {
                // si le noeud parent des marques est séléctioné
                if (TreeNodeSelected.Equals(AllBrandNode))
                {
                    // on charge les marques
                    LoadBrands();
                    // on les affiches
                    DisplayBrandDescription();
                }
                // si le noeud parent des familles est séléctionné
                else if (TreeNodeSelected.Equals(AllFamilyNode))
                {
                    // on chage les familles
                    LoadFamilies();
                    // on affiche les sous-familles
                    DisplayFamilyDescription();
                }
                else
                {
                    var NodeParent = TreeNodeSelected.Parent;
                    if (NodeParent.Equals(AllBrandNode))
                        // Une marque est séléctionnée
                    {
                        // on met à jour le filtre et on réaffiche les données
                        BrandFilter = (int?) TreeNodeSelected.Tag;
                        DisplayArticlesWithFilter();
                    }
                    else if (NodeParent.Equals(AllFamilyNode))
                        // Une famille est séléctionnée
                    {
                        // On charge les données de ses sous-familles et on affiches les données.
                        var FamilyId = ((int?) TreeNodeSelected.Tag).Value;
                        LoadSubFamily(TreeNodeSelected, FamilyId);
                        DisplaySubFamilyDescription(FamilyId);
                    }
                    else
                        // Une sous-famille est séléctionnée
                    {
                        // on met à jour le filtre et on réaffiche les données
                        SubFamilyFilter = (int?) TreeNodeSelected.Tag;
                        DisplayArticlesWithFilter();
                    }
                }
            }
            // On trie sur la première colonne
            SortColumn(0);
            listView1.EndUpdate();
            // On met à jour la barre de status
            UpdateStatusBar();
        }
        
        /// <summary>
        /// Met à jour la bare de status avec le nombre de ligne dans les différentes tables de la base
        /// </summary>
        private void UpdateStatusBar()
        {
            var CountArticle = DaoRegistery.GetInstance.DaoArticle.Count();
            var CountFamily = DaoRegistery.GetInstance.DaoFamille.Count();
            var CountBrand = DaoRegistery.GetInstance.DaoMarque.Count();
            var CountSubFamily = DaoRegistery.GetInstance.DaoSousFamille.Count();
            StatusText.Text = CountArticle + " articles, " + CountFamily + " familles, " + CountSubFamily + 
                              " sous-familles et " + CountBrand + " marques en base.";
        }

        /// <summary>
        /// Affiches les familles chargées dans le modèle
        /// </summary>
        private void DisplayFamilyDescription()
        {
            // On met à jour ce drapeau pour indiquer que l'on affiche des familles sur la droite de l'écran
            ArticleViewOn = ActiveList.Family;
            foreach (var Family in FamilyModel)
            {
                listView1.Items.Add(new ListViewItem(Family.Nom) { Tag =  Family.Id });
            }
        }
        
        /// <summary>
        /// Affiches les marques chargées dans le modèle
        /// </summary>
        private void DisplayBrandDescription()
        {
            // On met à jour ce drapeau pour indiquer que l'on affiche des marques sur la droite de l'écran
            ArticleViewOn = ActiveList.Brand;
            foreach (var Brand in BrandModel)
            {
                listView1.Items.Add(new ListViewItem(Brand.Nom) { Tag =  Brand.Id });
            }
        }
        
        /// <summary>
        /// Affiches les sous-familles chargées dans le modèle
        /// </summary>
        private void DisplaySubFamilyDescription(int Id)
        {
            // On met à jour ce drapeau pour indiquer que l'on affiche des sous-familles sur la droite de l'écran
            ArticleViewOn = ActiveList.Subfamily;
            foreach (var SubFamily in SubFamilyModel[Id])
            {
                listView1.Items.Add(new ListViewItem(SubFamily.Nom) { Tag =  SubFamily.Id });
            }
        }
        
        /// <summary>
        /// Affiches les articles chargés dans le modèle, si les filtres
        /// (<see cref="BrandFilter"/>, <see cref="SubFamilyFilter"/>) ne sont pas null, les articles doivent les passer
        /// pour être affichés.
        /// </summary>
        private void DisplayArticlesWithFilter()
        {
            // On met à jour ce drapeau pour indiquer que l'on affiche des articles sur la droite de l'écran
            ArticleViewOn = ActiveList.Article;
            // On s'assure que les colonnes sont bien présentes
            InserColonne();
            foreach (var Article in ArticlesModel)
            {
                //On test le filtre de la marque
                if (! BrandFilter.HasValue || BrandFilter.Equals(Article.Marque.Id))
                {
                    //On test le filtre de la sous-famille
                    if (! SubFamilyFilter.HasValue || SubFamilyFilter.Equals(Article.SousFamille.Id))
                    {
                        //Les filtres sont passés, on affiche l'article
                        listView1.Items.Add(
                            new ListViewItem(new [] {
                                Article.Description, 
                                Article.SousFamille.Famille.Nom,
                                Article.SousFamille.Nom, 
                                Article.Marque.Nom, 
                                Article.Quantite.ToString()
                            }) { Tag = Article.RefArticle });
                    }
                }
            }
        }

        /// <summary>
        /// Charge les articles de la base dans le modèle
        /// </summary>
        private void LoadArticles()
        {
            ArticlesModel = DaoRegistery.GetInstance.DaoArticle.GetAll();
        }
        
        /// <summary>
        /// Charge les marques de la base dans le modèle
        /// </summary>
        private void LoadBrands()
        {
            BrandModel = DaoRegistery.GetInstance.DaoMarque.GetAllMarques();
            // On charge les noeuds des marques
            AllBrandNode.Nodes.Clear();
            foreach (var TreeNode in BrandModel.Select(Marque => new TreeNode(Marque.Nom) { Tag = Marque.Id }))
            {
                AllBrandNode.Nodes.Add(TreeNode);
            }
            AllBrandNode.Expand();
        }

        /// <summary>
        /// Charge les Familles dans le modèle
        /// </summary>
        private void LoadFamilies()
        {
            FamilyModel = DaoRegistery.GetInstance.DaoFamille.GetAllFamilles();
            // On sauvegarde les sous-familles déjà chargées
            var SubFamiliesToLoad = new List<int>();
            foreach (TreeNode Node in AllFamilyNode.Nodes)
            {
                if (Node.Nodes.Count > 0)
                {
                    SubFamiliesToLoad.Add((int)Node.Tag);
                }
            }
            AllFamilyNode.Nodes.Clear();
            foreach (var Family in FamilyModel)
            {
                if (! Family.Id.HasValue) return;
                
                var SubNode = new TreeNode(Family.Nom) { Tag = Family.Id.Value };
                AllFamilyNode.Nodes.Add(SubNode);
                // Si la sous-famille était précédement chargé, on la recharge
                if (SubFamiliesToLoad.Contains(Family.Id.Value))
                {
                    LoadSubFamily(SubNode, Family.Id.Value);
                }
            }
            AllFamilyNode.Expand();
        }
        
        /// <summary>
        /// Charge les sous-amilles dans le modèle
        /// </summary>
        private void LoadSubFamily(TreeNode ParentNode, int FamilyId)
        {
            ParentNode.Nodes.Clear();
            if (SubFamilyModel.ContainsKey(FamilyId))
            {
                SubFamilyModel.Remove(FamilyId);
            }
            SubFamilyModel.Add(FamilyId, DaoRegistery.GetInstance.DaoSousFamille.GetSubFamilyOfFamily(FamilyId));
            foreach (var SubFamily in SubFamilyModel[FamilyId])
            {
                if (! SubFamily.Id.HasValue) return;
                ParentNode.Nodes.Add(new TreeNode(SubFamily.Nom) { Tag = SubFamily.Id });
            }
            ParentNode.Expand();
        }

        /// <summary>
        /// Tri la liste en fonction de la column d'index <paramref name="ColumnIndex"/>. Si la colonne est celle de la
        /// quantité, rien ne se passe
        /// </summary>
        /// <param name="ColumnIndex">Le numéro de la colonne sur laquelle trier</param>
        private void SortColumn(int ColumnIndex)
        {
            // si c'est la quantité on annule le tri
            if (ColumnIndex == COLUMN_QUANTITY)
            {
                return;
            }

            // On enlève le symbole ▼ de la dernière colonne trié (si elle existe)
            var SortedColumn = listView1.Columns[ColumnIndex];
            if (LastSortedColumn != null)
            {
                LastSortedColumn.Text = LastSortedColumn.Text.Substring(2, LastSortedColumn.Text.Length - 2);
            }

            // On créer des groupe alphabéthique en fonction de la colonne
            foreach (ListViewItem ListView1Item in listView1.Items)
            {
                var FirstLetter = ListView1Item.SubItems[ColumnIndex].Text.Substring(0, 1);
                var ListView1Group = listView1.Groups[FirstLetter.ToLower()];
                if (ListView1Group == null)
                {
                    listView1.Groups.Add(ListView1Group = new ListViewGroup(FirstLetter.ToLower(), FirstLetter.ToUpper()));
                }

                ListView1Item.Group = ListView1Group;
            }

            // On tri les groupe et on ajoute à la colonne le triangle indiquant le sens du tri
            var SortedGroup = new ListViewGroup[listView1.Groups.Count];
            listView1.Groups.CopyTo(SortedGroup, 0);
            listView1.Groups.Clear();
            var SortIcon = "";
            switch (listView1.Sorting)
            {
                case SortOrder.Ascending:
                    listView1.Groups.AddRange(SortedGroup.OrderBy(Group => Group.Name).ToArray());
                    SortIcon = "▲ ";
                    break;
                case SortOrder.Descending:
                    listView1.Groups.AddRange(SortedGroup.OrderByDescending(Group => Group.Name).ToArray());
                    SortIcon = "▼ ";
                    break;
            }
            SortedColumn.Text = SortIcon + SortedColumn.Text;
            
            listView1.Sort();
            LastSortedColumn = SortedColumn;
        }

        /// <summary>
        /// Recharge la liste en utilisant le dernier noeud séléctionné
        /// </summary>
        private void ReloadList()
        {
            if (LastTreeNodeSelected != null)
            {
                LoadCorrespondingList(LastTreeNodeSelected);
            }
        }

        /// <summary>
        /// Ouvre le formulaire d'ajout d'un objet dans une nouvelle fenêtre
        /// </summary>
        /// <returns><code>DialogResult</code> le resultat de la fenêtre</returns>
        private DialogResult OpenEditForm()
        {
            DialogResult Result;
            if (ActiveList.Article == ArticleViewOn)
            {
                var ArticleId = (string) listView1.FocusedItem.Tag;
                using (var AjoutFormulaire = new ArticleForm(ArticleId))
                {
                    AjoutFormulaire.StartPosition = FormStartPosition.CenterParent;
                    Result = AjoutFormulaire.ShowDialog(this);
                }
            }
            else
            {
                var Id = (int) listView1.FocusedItem.Tag;
                using (var AjoutFormulaire = new DefaultForm(ArticleViewOn, Id))
                {
                    AjoutFormulaire.StartPosition = FormStartPosition.CenterParent;
                    Result = AjoutFormulaire.ShowDialog(this);
                }
            }

            return Result;
        }

        private DialogResult OpenAddForm()
        {
            DialogResult Result;
            if (ActiveList.Article == ArticleViewOn)
            {
                using (var AjoutFormulaire = new ArticleForm())
                {
                    AjoutFormulaire.StartPosition = FormStartPosition.CenterParent;
                    Result = AjoutFormulaire.ShowDialog(this);
                }
            }
            else
            {
                using (var AjoutFormulaire = new DefaultForm(ArticleViewOn))
                {
                    AjoutFormulaire.StartPosition = FormStartPosition.CenterParent;
                    Result = AjoutFormulaire.ShowDialog(this);
                }
            }

            return Result;
        }
        
        /// <summary>
        /// Supprime l'élément passé en paramètre, après avoir validé une fenêtre de confirmation
        /// </summary>
        /// <param name="ItemToDelete">L'élément à supprimer</param>
        /// <returns><code>DialogResult</code> le resultat de la fenêtre de confirmation</returns>
        private DialogResult Delete(ListViewItem ItemToDelete)
        {
            var Result = MessageBox.Show("Voulez vous supprimer cet article ?", "Confirmation", 
                MessageBoxButtons.YesNo);
            if (Result == DialogResult.Yes)
            {
                try
                {
                    if (ActiveList.Article == ArticleViewOn)
                    {
                        var ArticleId = (string) ItemToDelete.Tag;
                        DaoRegistery.GetInstance.DaoArticle.Delete(ArticleId);
                        // Sur la suppression d'un article, on ne rafraîchi que la liste
                        var RemoveIndex = -1;
                        for (var Index = 0; Index < ArticlesModel.Count; Index++)
                        {
                            if (ArticleId.Equals(ArticlesModel[Index].RefArticle))
                            {
                                RemoveIndex = Index;
                                break;
                            }
                        }
                        if (RemoveIndex != -1)
                        {
                            ArticlesModel.RemoveAt(RemoveIndex);
                        }
                    }
                    else
                    {
                        var Id = (int) ItemToDelete.Tag;
                        switch (ArticleViewOn)
                        {
                            case ActiveList.Brand:
                            {
                                LastTreeNodeSelected = AllBrandNode;
                                DaoRegistery.GetInstance.DaoMarque.Delete(Id);
                                break;
                            }
                            case ActiveList.Family:
                            {
                                LastTreeNodeSelected = AllFamilyNode;
                                DaoRegistery.GetInstance.DaoFamille.Delete(Id);
                                break;
                            }
                            case ActiveList.Subfamily:
                            {
                                LastTreeNodeSelected = AllFamilyNode;
                                DaoRegistery.GetInstance.DaoSousFamille.Delete(Id);
                                break;
                            }
                        }
                        ReloadList();
                    }
                    listView1.Items.Remove(ItemToDelete);
                    UpdateStatusBar();
                }
                catch (Exception Error)
                {
                    MessageBox.Show("Cette opération a échouée : \n" + Error.Message, "Erreur");
                    Result = DialogResult.Abort;
                }
            }
            return Result;
        }


        //************************************************** EVENT **************************************************//
        
        /// <summary>
        /// Ouvre le menu contextuelle au clique droit
        /// Si aucun élément n'est séléctionné, on ne peut que ajouter un nouvel élément
        /// sinon on peut ajouter un nouvel élément, modifier l'élément séléctionné ou le supprimer
        /// </summary>
        /// <param name="Sender">Non utilisé</param>
        /// <param name="Event">Les données de l'événement de clique</param>
        private void View_MouseDown(object Sender, MouseEventArgs Event)
        {
            if (Event.Button == MouseButtons.Right)
            {
                if (listView1.SelectedItems.Count == 0)
                {
                    contextMenu.Items[1].Enabled = false;
                    contextMenu.Items[2].Enabled = false;
                }
                else
                {
                    contextMenu.Items[1].Enabled = true;
                    contextMenu.Items[2].Enabled = true;
                }
                contextMenu.Show(MousePosition);
            }
        }
        
        /// <summary>
        /// Charge dans la <b>ListView</b> la liste des articles, marques, familles ou sous-familles en
        /// fonction de l'objet selectioné dans la <b>TreeView</b>
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>TreeViewEventArgs</b> contient l'événement</param>
        private void treeView1_AfterSelect(object Sender, TreeViewEventArgs Event)
        {
            if (Event.Action == TreeViewAction.ByMouse || Event.Action == TreeViewAction.ByKeyboard)
            {
                var TreeNodeSelected = Event.Node;
                LoadCorrespondingList(TreeNodeSelected);
            }
        }
        
        /// <summary>
        /// Ouvre la fenêtre de suppression de base.
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void SupprimerLaBaseToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            var Result = MessageBox.Show("Cette action est irreversible, êtes vous certain de continuer ?",
                "Suppression de la base, confirmation", MessageBoxButtons.YesNoCancel);
            if (Result == DialogResult.Yes)
            {
                listView1.Items.Clear();
                AllBrandNode.Nodes.Clear();
                AllFamilyNode.Nodes.Clear();
                DaoRegistery.GetInstance.ClearAll();
                ReloadList();
            }
        }
        
        /// <summary>
        /// Sauvegarde la position de la fenêtre principale à la fermeture.
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>FormClosingEventArgs</b> contient l'événement</param>
        private void FormMain_FormClosing(object Sender, FormClosingEventArgs Event)
        {
            Settings.Default.Left = Left;
            Settings.Default.Top = Top;
            Settings.Default.Height = Height;
            Settings.Default.Width = Width;
            Settings.Default.Maximized = WindowState == FormWindowState.Maximized;
            Settings.Default.Save();
        }

        /// <summary>
        /// Apellé au chargement du formulaire, récupère les dernières dimensions de l'application, initialiser les vues
        /// et affiche toutes les articles
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>FormClosingEventArgs</b> contient l'événement</param>
        private void FormMain_Load(object Sender, EventArgs Event)
        {
            // On récupère les dernière dimensions de l'application
            Left = Settings.Default.Left;
            Top = Settings.Default.Top;
            Height = Settings.Default.Height;
            Width = Settings.Default.Width;
            var Maximized = Settings.Default.Maximized;
            if (Maximized)
            {
                WindowState = FormWindowState.Maximized;
            }
            // On créer les vues
            AllArticles = new TreeNode("Tous les articles");
            AllFamilyNode = new TreeNode("Familles");
            AllBrandNode = new TreeNode("Marques");
            treeView1.Nodes.AddRange(new [] {AllArticles, AllFamilyNode, AllBrandNode});
            treeView1.SelectedNode = AllArticles;
            LoadCorrespondingList(AllArticles);
        }
        
        /// <summary>
        /// Apellé au clique sur une colonnes de la liste, lance un tri sur cette colonne
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>FormClosingEventArgs</b> contient l'événement</param>
        private void listView1_ColumnClick(object Sender, ColumnClickEventArgs Event)
        {
            if (listView1.Sorting == SortOrder.Ascending)
            {
                listView1.Sorting = SortOrder.Descending;
            }
            else
            {
                listView1.Sorting = SortOrder.Ascending;
            }
            SortColumn(Event.Column);
        }

        /// <summary>
        /// Écoute les entrée clavier de l'utilisateur pour rafraichir la liste s'il appuie sur F5
        /// </summary>
        /// <param name="Message"><b>Message</b> inutilisé</param>
        /// <param name="KeyData"><b>Keys</b> contient la touche qui viens d'être actionné</param>
        /// <returns><c>true</c> si l'événement est consommé</returns>
        protected override bool ProcessCmdKey(ref Message Message, Keys KeyData)
        {
            if (KeyData == Keys.F5)
            {
                ReloadList();
                return true;
            }
            return base.ProcessCmdKey(ref Message, KeyData);
        }

        /// <summary>
        /// Apellé au clique sur une des actions du menu contextuel, selon l'élément cliqué cette fonction va
        /// ouvrir la fenetre d'ajout, celle d'édition, ou lancer la suppression de l'élément séléctionné.
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void contextMenu_Click(object Sender, ToolStripItemClickedEventArgs Event)
        {
            DialogResult Result;
            contextMenu.Hide();
            switch (Event.ClickedItem.Text)
            {
                case "Ajout":
                    Result = OpenAddForm();
                    break;
                case "Modification":
                    Result = OpenEditForm();
                    break;
                case "Suppression":
                    Result = Delete(listView1.FocusedItem);
                    break;
                default: Result = DialogResult.None; break;
            }
            if (Result == DialogResult.OK)
            {
                ReloadList();
            }
        }

        /// <summary>
        /// Apellée quand l'utilisateur clique sur le bouton actualiser du menu fichier, actualise la vue
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void ActualiserToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            ReloadList();
        }
        
        /// <summary>
        /// Ouvre la fenêtre d'import de base.
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void importerToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            using (var ImporterMenu = new ImporterMenu())
            {
                ImporterMenu.StartPosition = FormStartPosition.CenterParent;
                var Result = ImporterMenu.ShowDialog(this);
                if (Result == DialogResult.OK)
                {
                    ReloadList();
                }
            }
        }
        
        /// <summary>
        /// Ouvre la fenêtre d'export de base.
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void exporterToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            using (var ExporterMenu = new ExporterMenu())
            {
                ExporterMenu.StartPosition = FormStartPosition.CenterParent;
                ExporterMenu.ShowDialog(this);
            }
        }
        
        /// <summary>
        /// Apellé quand une touche clavier est appuyé avec alors que la liste a le focus, si la touche est 'Entrée' on
        /// édite l'élément séléctionné, si c'est 'Suppr' on le supprime.
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void listView1_KeyDown(object Sender, KeyEventArgs Event)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (Event.KeyCode == Keys.Delete)
                {
                    Delete(listView1.SelectedItems[0]);
                } else if (Event.KeyCode == Keys.Enter)
                {
                    OpenEditForm();
                }
            }
        }
        /// <summary>
        /// Apellé une élément de la liste est double-cliqué, on ouvre alors le menu d'édition
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void OnListDoubleClick(object Sender, EventArgs Event)
        {
            OpenEditForm();
        }
    }
    
    /// <summary>
    /// Le type de liste pouvant être affichée sur le coté droit de la fenêtre
    /// </summary>
    public enum ActiveList
    {
        //La liste des articles
        Article,
        //La liste des marques
        Brand,
        //La liste des familles
        Family,
        //La liste des sous-familles
        Subfamily,
        //Etat de la liste inconnu
        Unknown
    }
}
