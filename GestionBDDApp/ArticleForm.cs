using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestionBDDApp
{
    /// <summary>
    /// Fenêtre du formulaire d'ajout ou de modification pour un article.
    /// </summary>
    public partial class ArticleForm : Form
    {

        /// <summary>
        /// Article à modifier ou créer.
        /// </summary>
        private Articles Article;
        /// <summary>
        /// La liste des marques.
        /// </summary>
        private List<Marques> BrandModel = new List<Marques>();
        /// <summary>
        /// La liste des familles.
        /// </summary>
        private List<Family> FamilyModel = new List<Family>();
        /// <summary>
        /// L'ensemble de toutes les sous-familles.
        /// </summary>
        private Dictionary<int, List<SubFamily>> SubFamilyModel = new Dictionary<int, List<SubFamily>>();

        /// <summary>
        /// Crée la fênetre de création d'article.
        /// </summary>
        public ArticleForm()
        {
            // On récupère les données des listes et les affiche dans les <b>ComboBox</b>.
            InitializeComponent();
            LoadItems();
            SetTitle("Formulaire de création d'un article");
            DisplayItems();
        }

        /// <summary>
        /// Crée la fênetre de modification d'article.
        /// </summary>
        /// <param name="IdArticle">La <b>String</b> d'id de l'article.</param>
        public ArticleForm(string IdArticle)
        {
            // On récupère l'article.
            Article = DaoRegistry.GetInstance.ArticleDao.GetArticleById(IdArticle);

            // On récupère les données des listes et les affiche dans les <b>ComboBox</b>.
            InitializeComponent();
            LoadItems();
            SetTitle("Formulaire de modification d'un article");
            DisplayItems();
            
            ReferenceBox.Enabled = false;
            
            // On rempli le formulaire avec les informations de l'article.
            ReferenceBox.Text = Article.RefArticle;
            DescriptionBox.Text = Article.Description;
            QuantityBox.Value = Article.Quantity;
            PriceBox.Value = (decimal) Article.Price;
            BrandComboBox.Text = Article.Marque.Nom;
            FamillyComboBox.Text = Article.SubFamily.Family.Name;
            SubFamillyComboBox.Text = Article.SubFamily.Name;
        }

        private void SetTitle(string Title)
        {
            Text = Title;
        }
        
        private void LoadItems()
        {
            BrandModel = DaoRegistry.GetInstance.BrandDao.GetAllMarques();
            FamilyModel = DaoRegistry.GetInstance.FamilyDao.GetAllFamilles();
            SubFamilyModel = DaoRegistry.GetInstance.SubFamilyDao.GetAllSousFamilles()
                .GroupBy(SousFamille => SousFamille.Family.Id.Value)
                .ToDictionary(
                    SousFamille => SousFamille.Key, 
                    V => V.Select(F => F).ToList());
        }
        
        private void DisplayItems()
        {
            foreach (var Brand in BrandModel)
            {
                BrandComboBox.Items.Add(new ComboBoxItem(Brand.Nom, Brand));
            }
            foreach (var Family in FamilyModel)
            {
                FamillyComboBox.Items.Add(new ComboBoxItem(Family.Name, Family));
            }
        }

        /// <summary>
        /// Charge les sous-familles dans la <b>ComboBox</b> en fonction de la clé de la famille sélectionnée.
        /// </summary>
        /// <param name="SubFamilyKey">La clé de la famille selectionnée.</param>
        private void DisplaySubFamily(int SubFamilyKey)
        {
            // On nettoie la liste des sous-familles dans la <b>ComboBox</b> et on la re-rempli avec les sous-familles
            // de la famille selectionnée.
            SubFamillyComboBox.Items.Clear();
            foreach (var SubFamily in SubFamilyModel[SubFamilyKey])
            {
                SubFamillyComboBox.Items.Add(new ComboBoxItem(SubFamily.Name, SubFamily));
            }
        }
        
        
        //************************************************** EVENT **************************************************//
        
        /// <summary>
        /// Charge les sous-familles dans la <b>ComboBox</b> lorsque que l'on change l'état de la <b>ComboBox</b> des familles
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void Familles_SelectedIndexChanged(object Sender, EventArgs Event)
        {
            DisplaySubFamily(((Family)((ComboBoxItem)((ComboBox)Sender).SelectedItem).Value).Id.Value);
        }
        
        /// <summary>
        /// Annule la modification ou l'ajout en fermant la fenêtre. 
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void CancelButton_Click(object Sender, EventArgs Event)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Valide le formulaire s'il est rempli (affiche un message d'erreur sinon). 
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void ValidateButton_Click(object Sender, EventArgs Event)
        {
            // On vérifie que tous les champs sont remplis
            var ErrorBuilder = new StringBuilder();
            var Ref = ReferenceBox.Text.Trim();

            if (Ref.Length > 8 || Ref.Length < 5)
            {
                ErrorBuilder.AppendLine(
                    "La référence doit faire entre 5 et 8 caractères (espace avant et après non-inclus)");
            }
            
            if (BrandComboBox.SelectedIndex == -1)
            {
                ErrorBuilder.AppendLine("Il faut choisir une marque !");
            }

            if (SubFamillyComboBox.SelectedIndex == -1)
            {
                ErrorBuilder.AppendLine("Il faut choisir une sous famille !");
            }
            
            if (FamillyComboBox.SelectedIndex == -1)
            {
                ErrorBuilder.AppendLine("Il faut choisir une famille !");
            }

            if (PriceBox.Value < 0)
            {
                ErrorBuilder.AppendLine("Le prix ne peux pas être négatif");
            }
            
            if (PriceBox.Value > int.MaxValue)
            {
                ErrorBuilder.AppendLine("Le prix est trop élevé");
            }
            
            if (QuantityBox.Value < 0)
            {
                ErrorBuilder.AppendLine("La quantité ne peux pas être négative");
            }

            if (QuantityBox.Value > int.MaxValue)
            {
                ErrorBuilder.AppendLine("La quantité est trop élevée");
            }

            var Description = DescriptionBox.Text.Trim();
            if (Description.Length > 150 || Description.Length < 1)
            {
                ErrorBuilder.AppendLine(
                    "La description doit faire entre 1 et 150 caractères (espace avant et après non-inclus)");
            }

            var Error = ErrorBuilder.ToString();
            if (Error.Length == 0)
            {
                // Si il y a un article, on le modifie et l'enregistre
                if (Article != null)
                {
                    Article.Description = DescriptionBox.Text;
                    Article.Quantity = (int) QuantityBox.Value;
                    Article.Price = (float) PriceBox.Value;
                    Article.Marque = (Marques)((ComboBoxItem)BrandComboBox.SelectedItem).Value;
                    Article.SubFamily = (SubFamily)((ComboBoxItem)SubFamillyComboBox.SelectedItem).Value;
                    DaoRegistry.GetInstance.ArticleDao.Update(Article);
                }
                // Sinon on créer un article et on l'enregistre
                else
                {
                    //on évite la création de doublons
                    if (DaoRegistry.GetInstance.ArticleDao.GetArticleById(Ref) != null)
                    {
                        DialogResult = DialogResult.Cancel;
                        MessageBox.Show("Cette référence est déja utilisé par un article", "Erreur", MessageBoxButtons.OK);
                        return;
                    }
                    else
                    {
                        Article = new Articles(Ref, DescriptionBox.Text, 
                            (SubFamily)((ComboBoxItem)SubFamillyComboBox.SelectedItem).Value, 
                            (Marques)((ComboBoxItem)BrandComboBox.SelectedItem).Value, 
                            (float) PriceBox.Value, (int) QuantityBox.Value);
                        DaoRegistry.GetInstance.ArticleDao.Create(Article);
                    }
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            // On affiche un message d'erreur s'il y a des erreurs
            else
            {
                DialogResult = DialogResult.Cancel;
                MessageBox.Show("Les champs suivants sont en erreur dans le formulaire : \n\n" + Error + 
                                "\nCorriger ces erreurs avant de sauvegarder.", "Error", MessageBoxButtons.OK);
            }
        }
    }

    /// <summary>
    /// Classe qui prend un objet et une string pour garder cette objet dans les ComboBox et afficher la string. 
    /// </summary>
    public class ComboBoxItem
    {
        /// <summary>
        /// Texte à affiche dans la <b>ComboBox</b>. 
        /// </summary>
        private string Text { get; }
        /// <summary>
        /// Objet à sauvegarder dans la <b>ComboBox</b>. 
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Créer l'objet à sauvegarder dans la <b>ComboBox</b>.
        /// </summary>
        /// <param name="Text">Texte du ComboBoxItem à afficher</param>
        /// <param name="Value">Objet du ComboBoxItem</param>
        public ComboBoxItem(string Text, object Value)
        {
            this.Text = Text;
            this.Value = Value;
        }
        /// <summary>
        /// Override de la fonction ToString pour que le nom de l'objet soit affiché.
        /// </summary>
        public override string ToString()
        {
            return Text;
        }
    }
}