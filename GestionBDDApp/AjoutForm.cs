using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace GestionBDDApp
{
    public partial class AjoutForm : Form
    {
        public AjoutForm()
        {
            InitializeComponent();
            LoadItems();
            DisplayItems();
        }

        public AjoutForm(string IdArticle)
        {
            Article = DaoRegistery.GetInstance.DaoArticle.GetArticleById(IdArticle);
            InitializeComponent();
            LoadItems();
            DisplayItems();
            ReferenceBox.Text = Article.RefArticle;
            DescriptionBox.Text = Article.Description;
            QuantityBox.Value = Article.Quantite;
            PriceBox.Value = (decimal) Article.Prix;
            BrandComboBox.Text = Article.Marque.Nom;
            FamillyComboBox.Text = Article.SousFamille.Famille.Nom;
            SubFamillyComboBox.Text = Article.SousFamille.Nom;
        }

        private Articles Article = null;
        private List<Marques> BrandModel = new List<Marques>();
        private List<Familles> FamillyModel = new List<Familles>();
        private Dictionary<int?, List<SousFamilles>> SubFamilyModel = new Dictionary<int?, List<SousFamilles>>();
        private void LoadItems()
        {
            BrandModel = DaoRegistery.GetInstance.DaoMarque.GetAllMarques();
            FamillyModel = DaoRegistery.GetInstance.DaoFamille.GetAllFamilles();
            SubFamilyModel = DaoRegistery.GetInstance.DaoSousFamille.GetAllSousFamilles()
                .GroupBy(SousFamille => SousFamille.Famille.Id)
                .ToDictionary(SousFamille => SousFamille.Key, v => v.Select(f => f).ToList());
        }

        private void Familles_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplaySubFamilly(((ComboBox)sender).SelectedIndex + 1);
        }

        private void DisplayItems()
        {
            foreach (var Brand in BrandModel)
            {
                BrandComboBox.Items.Add(new ComboBoxItem(Brand.Nom, Brand));
            }
            foreach (var Familly in FamillyModel)
            {
                FamillyComboBox.Items.Add(new ComboBoxItem(Familly.Nom, Familly));
            }
        }


        private void DisplaySubFamilly(int SubFamillyKey)
        {
            SubFamillyComboBox.Items.Clear();
            foreach (var SubFamilly in SubFamilyModel[SubFamillyKey])
            {
                SubFamillyComboBox.Items.Add(new ComboBoxItem(SubFamilly.Nom, SubFamilly));
            }
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ValidateButton_Click(object sender, EventArgs e)
        {
            if(BrandComboBox.SelectedIndex != -1 && FamillyComboBox.SelectedIndex != -1 && SubFamillyComboBox.SelectedIndex != -1 && DescriptionBox.TextLength > 0 && PriceBox.Value >= 0 && QuantityBox.Value >= 0)
            {
                if (Article != null)
                {
                    //TODO
                    Console.WriteLine("Modifcation de : " + Article.Description);
                }
                else
                {

                    Article = new Articles(ReferenceBox.Text, DescriptionBox.Text, (SousFamilles)((ComboBoxItem)SubFamillyComboBox.SelectedItem).Value, (Marques)((ComboBoxItem)BrandComboBox.SelectedItem).Value, (float) PriceBox.Value, (int) QuantityBox.Value);
                    DaoRegistery.GetInstance.DaoArticle.create(Article);
                    Console.WriteLine("Création de : " + Article.Description);
                }
                this.Close();
            }
            else
                MessageBox.Show("Veillez remplir tous les champs", "Error", MessageBoxButtons.OK);
        }
    }
    public class ComboBoxItem
    {
        public string Text { get; }
        public object Value { get; }

        public ComboBoxItem(string Text, object Value)
        {
            this.Text = Text;
            this.Value = Value;
        }
        public override string ToString()
        {
            return Text;
        }
    }
}