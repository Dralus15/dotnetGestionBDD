﻿using System;
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
                ImporterMenu.ShowDialog(this);
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

        private TreeNode AllArticles;
        private TreeNode AllFamilyNode;
        private TreeNode AllBrandNode;
        
        private TreeNode LastTreeNodeSelected;

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
            LastTreeNodeSelected = TreeNodeSelected;
            BrandFilter = null;
            SubFamilyFilter = null;
            SupprColonne();
            listView1.Groups.Clear();
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
            UpdateStatusBar();
        }

        private void supprimerLaBaseToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            DaoRegistery.GetInstance.ClearAll();
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
            var CountSubFamily = 0;
            foreach (var SubFamily in SubFamilyModel.Values)
            {
                CountSubFamily += SubFamily.Count;
            }
            StatusText.Text = ArticlesModel.Count + " articles, " + FamilyModel.Count + " familles, " + CountSubFamily + " sous-familles et " + BrandModel.Count + " marques en base.";
        }

        private int? SubFamilyFilter = null;
        private int? BrandFilter = null;


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
                    if (!SubFamilyFilter.HasValue || SubFamilyFilter.Equals(Article.Marque.Id))
                    {
                        listView1.Items.Add(new ListViewItem(new [] {
                            Article.Description, 
                            Article.SousFamille.Famille.Nom,
                            Article.SousFamille.Nom, 
                            Article.Marque.Nom, 
                            Article.Quantite.ToString()}) { Tag = Article.RefArticle });
                    }
                }
            }
        }

        private void LoadArticles()
        {
            ArticlesModel = DaoRegistery.GetInstance.DaoArticle.getAll();
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
                .ToDictionary(SousFamille => SousFamille.Key, v => v.Select(f => f).ToList());
            FamilyModel = DaoRegistery.GetInstance.DaoFamille.GetAllFamilles();
            //On sauvegarde les sous familles chargés
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
                    listView1.Groups.Add(ListView1Group = new ListViewGroup(FirstLetter.ToLower(), FirstLetter));
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
            }
            return base.ProcessCmdKey(ref Message, KeyData);
        }

        private void actualiserToolStripMenuItem_Click(object Sender, EventArgs Event)
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

        private void FormMain_KeyDown(object Sender, KeyEventArgs KeyEvent)
        {
            if (KeyEvent.KeyCode == Keys.Delete && listView1.SelectedItems.Count > 0)
            {
                Delete(listView1.SelectedItems[0]);
            }
        }

        private void Delete(ListViewItem ItemToDelete)
        {
            if (ActiveList.Article == ArticleViewOn)
            {
                string ArticleId = (string) ItemToDelete.Tag;
                DaoRegistery.GetInstance.DaoArticle.delete(ArticleId);
            }
            else
            {
                int Id = (int) listView1.Tag;
                switch (ArticleViewOn)
                {
                    case ActiveList.Brand:
                    {
                        DaoRegistery.GetInstance.DaoMarque.delete(Id);
                        break;
                    }
                    case ActiveList.Family:
                    {
                        DaoRegistery.GetInstance.DaoFamille.delete(Id);
                        break;
                    }
                    case ActiveList.Subfamily:
                    {
                        DaoRegistery.GetInstance.DaoSousFamille.delete(Id);
                        break;
                    }
                }
            }
            listView1.Items.Remove(ItemToDelete);
        }
    }
}
