﻿using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GestionBDDApp
{
    /// <summary>
    /// Fenêtre du formulaire d'ajout ou de modification pour un article.
    /// </summary>
    public partial class ArticleForm : Form
    {

        /// <summary>
        /// Article à modifier ou créer
        /// </summary>
        private Articles Article;
        /// <summary>
        /// La liste des marques
        /// </summary>
        private List<Marques> BrandModel = new List<Marques>();
        /// <summary>
        /// La liste des familles
        /// </summary>
        private List<Familles> FamilyModel = new List<Familles>();
        /// <summary>
        /// L'ensemble de toutes les sous-familles
        /// </summary>
        private Dictionary<int, List<SousFamilles>> SubFamilyModel = new Dictionary<int, List<SousFamilles>>();

        /// <summary>
        /// Créer la fênetre de création d'article.
        /// </summary>
        public ArticleForm()
        {
            // On récupère les données des listes et les affiche dans les <b>ComboBox</b>
            InitializeComponent();
            LoadItems();
            SetTitle("Formulaire de création d'un article");
            DisplayItems();
        }

        /// <summary>
        /// Créer la fênetre de modification d'article.
        /// </summary>
        /// <param name="IdArticle">La <b>String</b> d'id de l'article</param>
        public ArticleForm(string IdArticle)
        {
            // On récupère l'article
            Article = DaoRegistery.GetInstance.DaoArticle.GetArticleById(IdArticle);

            // On récupère les données des listes et les affiche dans les <b>ComboBox</b>
            InitializeComponent();
            LoadItems();
            SetTitle("Formulaire de modification d'un article");
            DisplayItems();
            
            // On rempli le formulaire avec les informations de l'article
            ReferenceBox.Text = Article.RefArticle;
            DescriptionBox.Text = Article.Description;
            QuantityBox.Value = Article.Quantite;
            PriceBox.Value = (decimal) Article.Prix;
            BrandComboBox.Text = Article.Marque.Nom;
            FamillyComboBox.Text = Article.SousFamille.Famille.Nom;
            SubFamillyComboBox.Text = Article.SousFamille.Nom;
        }

        private void SetTitle(string Title)
        {
            Text = Title;
        }
        
        private void LoadItems()
        {
            BrandModel = DaoRegistery.GetInstance.DaoMarque.GetAllMarques();
            FamilyModel = DaoRegistery.GetInstance.DaoFamille.GetAllFamilles();
            SubFamilyModel = DaoRegistery.GetInstance.DaoSousFamille.GetAllSousFamilles()
                .GroupBy(SousFamille => SousFamille.Famille.Id.Value)
                .ToDictionary(SousFamille => SousFamille.Key, V => V.Select(F => F).ToList());
        }
        
        private void DisplayItems()
        {
            foreach (var Brand in BrandModel)
            {
                BrandComboBox.Items.Add(new ComboBoxItem(Brand.Nom, Brand));
            }
            foreach (var Familly in FamilyModel)
            {
                FamillyComboBox.Items.Add(new ComboBoxItem(Familly.Nom, Familly));
            }
        }

        /// <summary>
        /// Charge les sous-familles dans la <b>ComboBox</b> en fonction de la clé de la famille sélectionné.
        /// </summary>
        /// <param name="SubFamillyKey">La clé de la famille selectionnée</param>
        private void DisplaySubFamilly(int SubFamillyKey)
        {
            // On nettoye la liste des sous-famille dans la <b>ComboBox</b> et on la re-rempli avec les sous-familles de la famille selectionné
            SubFamillyComboBox.Items.Clear();
            foreach (var SubFamilly in SubFamilyModel[SubFamillyKey])
            {
                SubFamillyComboBox.Items.Add(new ComboBoxItem(SubFamilly.Nom, SubFamilly));
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
            DisplaySubFamilly(((Familles)((ComboBoxItem)((ComboBox)Sender).SelectedItem).Value).Id.Value);
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
            // On vérifie que tous les champs sont remplis //TODO better verification + deduplication
            if (BrandComboBox.SelectedIndex != -1 && FamillyComboBox.SelectedIndex != -1 
                    && SubFamillyComboBox.SelectedIndex != -1 && DescriptionBox.TextLength > 0 && PriceBox.Value >= 0 
                    && QuantityBox.Value >= 0)
            {
                // Si il y a un article, on le modifie et l'enregistre
                if (Article != null)
                {
                    Article.Description = DescriptionBox.Text;
                    Article.Quantite = (int) QuantityBox.Value;
                    Article.Prix = (float) PriceBox.Value;
                    Article.Marque = (Marques)((ComboBoxItem)BrandComboBox.SelectedItem).Value;
                    Article.SousFamille = (SousFamilles)((ComboBoxItem)SubFamillyComboBox.SelectedItem).Value;
                    DaoRegistery.GetInstance.DaoArticle.Update(Article);
                    Console.WriteLine("Modifcation de : " + Article.Description);
                }
                // Sinon on créer un article et on l'enregistre
                else
                {
                    Article = new Articles(ReferenceBox.Text, DescriptionBox.Text, (SousFamilles)((ComboBoxItem)SubFamillyComboBox.SelectedItem).Value, (Marques)((ComboBoxItem)BrandComboBox.SelectedItem).Value, (float) PriceBox.Value, (int) QuantityBox.Value);
                    DaoRegistery.GetInstance.DaoArticle.Create(Article);
                    Console.WriteLine("Création de : " + Article.Description);
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            // On affiche un message d'erreur s'il y a un champ vide
            else
                MessageBox.Show("Veillez remplir tous les champs", "Error", MessageBoxButtons.OK);
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
        public string Text { get; }
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