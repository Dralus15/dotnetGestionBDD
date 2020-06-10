using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using GestionBDDApp.Properties;

namespace GestionBDDApp
{
    public partial class FormMain : Form
    {
        
        public FormMain()
        {
            InitializeComponent();
        }

        private void importerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImporterMenu ImporterMenu = new ImporterMenu())
            {
                ImporterMenu.StartPosition = FormStartPosition.CenterParent;
                ImporterMenu.ShowDialog(this);
            }
        }
        
        private void exporterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ExporterMenu ExporterMenu = new ExporterMenu())
            {
                ExporterMenu.StartPosition = FormStartPosition.CenterParent;
                ExporterMenu.ShowDialog(this);
            }
        }

        private TreeNode AllArticles;
        private TreeNode AllFamilyNode;
        private TreeNode AllBrandNode;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.ByKeyboard)
            {
                Console.WriteLine("Charging: " + e.Node.Name + " " + e.Node.Text + " " + e.Node.Index);
                MarquesChoosed = null;
                SousFamillesChoosed = null;
                SupprColonne();
                if (e.Node.Equals(AllArticles))
                {
                    LoadModel();
                    DisplayArticlesWithFilter();
                }
                else
                {
                    if (e.Node.Equals(AllBrandNode))
                    {
                        loadMarques();
                    }
                    else if (e.Node.Equals(AllFamilyNode))
                    {
                        loadFamilies();
                    }
                    else
                    {
                        var NodeParent = e.Node.Parent;
                        if (NodeParent.Equals(AllBrandNode))
                        // Une marque est séléctionnée
                        {
                            MarquesChoosed = (int?) e.Node.Tag;
                            DisplayArticlesWithFilter();
                        } else 
                        if (NodeParent.Equals(AllFamilyNode))
                        // Une famille est séléctionnée
                        {
                            loadSubFamily(e.Node, ((int?) e.Node.Tag).Value);
                            DescriptionModel = e.Node.Text;
                            DisplayDescription();
                        }
                        else
                        // Une sous-famille est séléctionnée
                        {
                            SousFamillesChoosed = (int?) e.Node.Tag;
                            DisplayArticlesWithFilter();
                        }
                    }
                }
            }
        }

        private void supprimerLaBaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DaoRegistery.GetInstance.clearAll();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Left = Left;
            Settings.Default.Top = Top;
            Settings.Default.Height = Height;
            Settings.Default.Width = Width;
            Settings.Default.Maximized = WindowState == FormWindowState.Maximized;
            Settings.Default.Save();
        }

        private void FormMain_Load(object sender, EventArgs e)
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
            LoadModel();
        }

        private List<Articles> ArticlesModel;
        private Dictionary<int?, List<SousFamilles>> SousFamillesModel;
        private List<Familles> FamillesModel;
        private List<Marques> MarquesModel;

        private int? SousFamillesChoosed = null;
        private int? MarquesChoosed = null;

        private string DescriptionModel = null;

        private void DisplayDescription()
        {
            listView1.Items.Clear();
            listView1.Items.Add(new ListViewItem(new[] {DescriptionModel}));
        }
        
        private void DisplayArticlesWithFilter()
        {
            InserColonne();
            listView1.Items.Clear();
            foreach (var Article in ArticlesModel)
            {
                if (! MarquesChoosed.HasValue || MarquesChoosed.Equals(Article.Marque.Id))
                {
                    if (!SousFamillesChoosed.HasValue || SousFamillesChoosed.Equals(Article.Marque.Id))
                    {
                        listView1.Items.Add(new ListViewItem(new [] {
                            Article.Description, 
                            Article.SousFamille.Famille.Nom,
                            Article.SousFamille.Nom, 
                            Article.Marque.Nom, 
                            Article.Quantite.ToString()}));
                    }
                }
            }
        }

        public void LoadModel()
        {
            ArticlesModel = DaoRegistery.GetInstance.DaoArticle.getAll();
        }

        public void loadMarques()
        {
            MarquesModel = DaoRegistery.GetInstance.DaoMarque.GetAllMarques();
            AllBrandNode.Nodes.Clear();
            foreach (var Marque in MarquesModel)
            {
                var TreeNode = new TreeNode(Marque.Nom);
                TreeNode.Tag = Marque.Id;
                AllBrandNode.Nodes.Add(TreeNode);
            }
        }

        private void loadFamilies()
        {
            SousFamillesModel = DaoRegistery.GetInstance.DaoSousFamille.GetAllSousFamilles()
                .GroupBy(c => c.Famille.Id)
                .ToDictionary(k => k.Key, v => v.Select(f => f).ToList());
            FamillesModel = DaoRegistery.GetInstance.DaoFamille.GetAllFamilles();
            AllFamilyNode.Nodes.Clear();//TODO clear les sous familles ? voulu ?
            foreach (var Famille in FamillesModel)
            {
                var subNode = new TreeNode(Famille.Nom);
                subNode.Tag = Famille.Id;
                AllFamilyNode.Nodes.Add(subNode);
            }
        }

        private void loadSubFamily(TreeNode parent, int familyId)
        {
            parent.Nodes.Clear();
            foreach (var SousFamille in SousFamillesModel[familyId])
            {
                var TreeNode = new TreeNode(SousFamille.Nom);
                TreeNode.Tag = SousFamille.Id;
                parent.Nodes.Add(TreeNode);
            }
        }
        private int sortColumn = -1;
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                if (listView1.Sorting == SortOrder.Ascending)
                {
                    listView1.Sorting = SortOrder.Descending;
                }
                else
                {
                    listView1.Sorting = SortOrder.Ascending;
                }
            }
            listView1.Sort();
        }
    }
}
