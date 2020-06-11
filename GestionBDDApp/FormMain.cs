using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using GestionBDDApp.Properties;

namespace GestionBDDApp
{
    public enum ActiveList
    {
        Article,
        Brand,
        Family,
        Subfamily,
        Unknown
    }
    public partial class FormMain : Form
    {
        
        public FormMain()
        {
            InitializeComponent();
        }

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
        
        private void exporterToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            using (var ExporterMenu = new ExporterMenu())
            {
                ExporterMenu.StartPosition = FormStartPosition.CenterParent;
                ExporterMenu.ShowDialog(this);
            }
        }

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

        private void treeView1_AfterSelect(object Sender, TreeViewEventArgs Event)
        {
            if (Event.Action == TreeViewAction.ByMouse || Event.Action == TreeViewAction.ByKeyboard)
            {
                var TreeNodeSelected = Event.Node;
                LoadCorrespondingList(TreeNodeSelected);
            }
        }

        private void LoadCorrespondingList(TreeNode TreeNodeSelected)
        {
            listView1.BeginUpdate();
            BrandFilter = null;
            SubFamilyFilter = null;
            if (LastTreeNodeSelected != TreeNodeSelected)
            {
                SupprColonne();
            }
            LastTreeNodeSelected = TreeNodeSelected;
            listView1.Groups.Clear();
            listView1.SelectedItems.Clear();
            if (LastTreeNodeSelected == null) { }
            if (TreeNodeSelected.Equals(AllArticles))
            {
                LoadArticles();
                DisplayArticlesWithFilter();
            }
            else
            {
                if (TreeNodeSelected.Equals(AllBrandNode))
                {
                    LoadBrands();
                    DisplayBrandDescription();
                }
                else if (TreeNodeSelected.Equals(AllFamilyNode))
                {
                    LoadFamilies();
                    DisplayFamilyDescription();
                }
                else
                {
                    var NodeParent = TreeNodeSelected.Parent;
                    if (NodeParent.Equals(AllBrandNode))
                        // Une marque est séléctionnée
                    {
                        BrandFilter = (int?) TreeNodeSelected.Tag;
                        DisplayArticlesWithFilter();
                    }
                    else if (NodeParent.Equals(AllFamilyNode))
                        // Une famille est séléctionnée
                    {
                        var FamilyId = ((int?) TreeNodeSelected.Tag).Value;
                        LoadSubFamily(TreeNodeSelected, FamilyId);
                        DisplaySubFamilyDescription(FamilyId);
                    }
                    else
                        // Une sous-famille est séléctionnée
                    {
                        SubFamilyFilter = (int?) TreeNodeSelected.Tag;
                        DisplayArticlesWithFilter();
                    }
                }
            }
            SortColumn(0);
            listView1.EndUpdate();
            UpdateStatusBar();
        }

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

        private void FormMain_FormClosing(object Sender, FormClosingEventArgs Event)
        {
            Settings.Default.Left = Left;
            Settings.Default.Top = Top;
            Settings.Default.Height = Height;
            Settings.Default.Width = Width;
            Settings.Default.Maximized = WindowState == FormWindowState.Maximized;
            Settings.Default.Save();
        }

        private void FormMain_Load(object Sender, EventArgs Event)
        {
            Left = Settings.Default.Left;
            Top = Settings.Default.Top;
            Height = Settings.Default.Height;
            Width = Settings.Default.Width;
            var Maximized = Settings.Default.Maximized;
            if (Maximized)
            {
                WindowState = FormWindowState.Maximized;
            }
            AllArticles = new TreeNode("Tous les articles");
            AllFamilyNode = new TreeNode("Familles");
            AllBrandNode = new TreeNode("Marques");
            treeView1.Nodes.AddRange(new [] {AllArticles, AllFamilyNode, AllBrandNode});
            treeView1.SelectedNode = AllArticles;
            LoadCorrespondingList(AllArticles);
        }

        private List<Articles> ArticlesModel = new List<Articles>();
        private Dictionary<int?, List<SousFamilles>> SubFamilyModel = new Dictionary<int?, List<SousFamilles>>();
        private List<Familles> FamilyModel = new List<Familles>();
        private List<Marques> BrandModel = new List<Marques>();

        private void UpdateStatusBar()
        {
            var CountArticle = DaoRegistery.GetInstance.DaoArticle.Count();
            var CountFamily = DaoRegistery.GetInstance.DaoFamille.Count();
            var CountBrand = DaoRegistery.GetInstance.DaoMarque.Count();
            var CountSubFamily = DaoRegistery.GetInstance.DaoSousFamille.Count();
            StatusText.Text = CountArticle + " articles, " + CountFamily + " familles, " + CountSubFamily + " sous-familles et " + CountBrand + " marques en base.";
        }

        private int? SubFamilyFilter;
        private int? BrandFilter;


        private ActiveList ArticleViewOn = ActiveList.Unknown;

        private void DisplayFamilyDescription()
        {
            ArticleViewOn = ActiveList.Family;
            listView1.Items.Clear();
            foreach (var Family in FamilyModel)
            {
                listView1.Items.Add(new ListViewItem(Family.Nom) { Tag =  Family.Id });
            }
        }

        private void DisplayBrandDescription()
        {
            ArticleViewOn = ActiveList.Brand;
            listView1.Items.Clear();
            foreach (var Brand in BrandModel)
            {
                listView1.Items.Add(new ListViewItem(Brand.Nom) { Tag =  Brand.Id });
            }
        }

        private void DisplaySubFamilyDescription(int Id)
        {
            ArticleViewOn = ActiveList.Subfamily;
            listView1.Items.Clear();
            foreach (var SubFamily in SubFamilyModel[Id])
            {
                listView1.Items.Add(new ListViewItem(SubFamily.Nom) { Tag =  SubFamily.Id });
            }
        }
        
        private void DisplayArticlesWithFilter()
        {
            ArticleViewOn = ActiveList.Article;
            InserColonne();
            listView1.Items.Clear();
            foreach (var Article in ArticlesModel)
            {
                if (! BrandFilter.HasValue || BrandFilter.Equals(Article.Marque.Id))
                {
                    if (! SubFamilyFilter.HasValue || SubFamilyFilter.Equals(Article.SousFamille.Id))
                    {
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

        private void LoadArticles()
        {
            ArticlesModel = DaoRegistery.GetInstance.DaoArticle.GetAll();
        }

        private void LoadBrands()
        {
            BrandModel = DaoRegistery.GetInstance.DaoMarque.GetAllMarques();
            AllBrandNode.Nodes.Clear();
            foreach (var TreeNode in BrandModel.Select(Marque => new TreeNode(Marque.Nom) { Tag = Marque.Id }))
            {
                AllBrandNode.Nodes.Add(TreeNode);
            }
            AllBrandNode.Expand();
        }

        private void LoadFamilies()
        {
            SubFamilyModel = DaoRegistery.GetInstance.DaoSousFamille.GetAllSousFamilles()
                .GroupBy(SousFamille => SousFamille.Famille.Id)
                .ToDictionary(SousFamille => SousFamille.Key, 
                    Grouping => Grouping.Select(SousFamille => SousFamille)
                    .ToList());
            FamilyModel = DaoRegistery.GetInstance.DaoFamille.GetAllFamilles();
            //On sauvegarde les sous familles chargées
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
                if (SubFamiliesToLoad.Contains(Family.Id.Value))
                {
                    LoadSubFamily(SubNode, Family.Id.Value);
                }
            }
            AllFamilyNode.Expand();
        }

        private void LoadSubFamily(TreeNode ParentNode, int FamilyId)
        {
            ParentNode.Nodes.Clear();
            foreach (var SubFamily in SubFamilyModel[FamilyId])
            {
                if (! SubFamily.Id.HasValue) return;
                ParentNode.Nodes.Add(new TreeNode(SubFamily.Nom) { Tag = SubFamily.Id });
            }
            ParentNode.Expand();
        }

        private ColumnHeader LastSortedColumn;
        private const int COLUMN_QUANTITY = 4;

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

        private void SortColumn(int ColumnIndex)
        {
            if (ColumnIndex == COLUMN_QUANTITY)
            {
                return;
            }

            var SortedColumn = listView1.Columns[ColumnIndex];
            if (LastSortedColumn != null)
            {
                LastSortedColumn.Text = LastSortedColumn.Text.Substring(2, LastSortedColumn.Text.Length - 2);
            }

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

        protected override bool ProcessCmdKey(ref Message Message, Keys KeyData)
        {
            if (KeyData == Keys.F5)
            {
                ReloadList();
                return true;
            }
            return base.ProcessCmdKey(ref Message, KeyData);
        }

        private void ActualiserToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            ReloadList();
        }

        private void ReloadList()
        {
            if (LastTreeNodeSelected != null)
            {
                LoadCorrespondingList(LastTreeNodeSelected);
            }
        }

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

        private DialogResult OpenEditForm()
        {
            DialogResult Result;
            if (ActiveList.Article == ArticleViewOn)
            {
                var ArticleId = (string) listView1.FocusedItem.Tag;
                using (var AjoutFormulaire = new AjoutForm(ArticleId))
                {
                    AjoutFormulaire.StartPosition = FormStartPosition.CenterParent;
                    Result = AjoutFormulaire.ShowDialog(this);
                }
            }
            else
            {
                var Id = (int) listView1.FocusedItem.Tag;
                using (var AjoutFormulaire = new AjoutFormAutre(ArticleViewOn, Id))
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
                using (var AjoutFormulaire = new AjoutForm())
                {
                    AjoutFormulaire.StartPosition = FormStartPosition.CenterParent;
                    Result = AjoutFormulaire.ShowDialog(this);
                }
            }
            else
            {
                using (var AjoutFormulaire = new AjoutFormAutre(ArticleViewOn))
                {
                    AjoutFormulaire.StartPosition = FormStartPosition.CenterParent;
                    Result = AjoutFormulaire.ShowDialog(this);
                }
            }

            return Result;
        }

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
                    }
                    else
                    {
                        var Id = (int) ItemToDelete.Tag;
                        switch (ArticleViewOn)
                        {
                            case ActiveList.Brand:
                            {
                                DaoRegistery.GetInstance.DaoMarque.Delete(Id);
                                break;
                            }
                            case ActiveList.Family:
                            {
                                DaoRegistery.GetInstance.DaoFamille.Delete(Id);
                                break;
                            }
                            case ActiveList.Subfamily:
                            {
                                DaoRegistery.GetInstance.DaoSousFamille.Delete(Id);
                                break;
                            }
                        }
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

        private void listView1_KeyDown(object Sender, KeyEventArgs KeyEvent)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (KeyEvent.KeyCode == Keys.Delete)
                {
                    Delete(listView1.SelectedItems[0]);
                } else if (KeyEvent.KeyCode == Keys.Enter)
                {
                    OpenEditForm();
                }
            }
        }

        private void OnListDoubleClick(object Sender, EventArgs Event)
        {
            OpenEditForm();
        }
    }
}
