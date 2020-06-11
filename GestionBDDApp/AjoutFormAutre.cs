using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using System;
using System.Windows.Forms;

namespace GestionBDDApp
{
    /// <summary>
    /// Fenêtre du formulaire d'ajout ou de modification pour une marque, une famille ou une sous-famille. 
    /// </summary>
    public partial class AjoutFormAutre : Form
    {
        /// <summary>
        /// Id de l'objet
        /// </summary>
        private int? Id;
        /// <summary>
        /// L'objet Marques
        /// </summary>
        private Marques Brand;
        /// <summary>
        /// L'objet Familles
        /// </summary>
        private Familles Family;
        /// <summary>
        /// L'objet SousFamilles
        /// </summary>
        private SousFamilles SubFamily;
        /// <summary>
        /// Type de l'objet à modifier ou créer
        /// </summary>
        private ActiveList Type;

        /// <summary>
        /// Créer la fenêtre de création d'une marque, d'une famille ou d'une sous-familles.
        /// </summary>
        public AjoutFormAutre(ActiveList Type) //TODO encapsulation
        {
            InitializeComponent();
            Text = "Formulaire de création";
            this.Type = Type;
            Id = null;
            // On Initialise les valeurs des champs du formulaire
            Initialize();
        }

        /// <summary>
        /// Créer la fenêtre de modification d'une marque, d'une famille ou d'une sous-familles.
        /// </summary>
        public AjoutFormAutre(ActiveList Type, int? Id)
        {
            InitializeComponent();
            Text = "Formulaire de modification";
            this.Type = Type;
            this.Id = Id;
            // On Initialise les valeurs des champs du formulaire
            Initialize();
        }

        /// <summary>
        /// Initialise les valeurs des champs de la fenêtre de modification
        /// </summary>
        private void Initialize()
        {
            switch (Type)
            {
                case ActiveList.Brand:
                {
                    // Si l'id a une valeur, on récupère la marque
                    if (Id.HasValue)
                    {
                        Brand = DaoRegistery.GetInstance.DaoMarque.GetMarqueById(Id.Value);
                        NameBox.Text = Brand.Nom;
                    }

                    break;
                }
                case ActiveList.Family:
                {
                    // Si l'id a une valeur, on récupère la famille
                    if (Id.HasValue)
                    {
                        Family = DaoRegistery.GetInstance.DaoFamille.GetFamilleById(Id.Value);
                        NameBox.Text = Family.Nom;
                    }

                    break;
                }
                case ActiveList.Subfamily:
                {
                    // On récupère toutes les familles pour les metttre dans la ComboBox des familles
                    foreach (var Family in DaoRegistery.GetInstance.DaoFamille.GetAllFamilles())
                    {
                        FamillyComboBox.Items.Add(new ComboBoxItem(Family.Nom, Family));
                    }
                    // Si l'id a une valeur, on récupère la sous-famille
                    if (Id.HasValue)
                    {
                        SubFamily = DaoRegistery.GetInstance.DaoSousFamille.GetSousFamilleById(Id.Value);
                        NameBox.Text = SubFamily.Nom;
                        FamillyComboBox.Text = SubFamily.Famille.Nom;
                    }
                    // On affiche la ComboBox des familles
                    FamillyComboBox.Visible = true;
                    NameLabel.Visible = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Valide le formulaire s'il est rempli (affiche un message d'erreur sinon) pour créer ou modifier un objet. 
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void ValidateButton_Click(object Sender, EventArgs Event)
        {
            // On vérifie que le champs de description est rempli
            if ( NameBox.TextLength > 0 )
            {
                switch (Type)
                {
                    case ActiveList.Brand:
                        {
                            // Si l'id a une valeur, on modifie le nom de la marque
                            if (Id.HasValue)
                            {
                                Brand.Nom = NameBox.Text;
                            }
                            // Sinon, on crée la marque
                            else
                            {
                                Brand = new Marques(null, NameBox.Text);
                            }
                            DaoRegistery.GetInstance.DaoMarque.Save(Brand);
                            break;
                        }
                    case ActiveList.Family:
                        {
                            // Si l'id a une valeur, on modifie le nom de la famille
                            if (Id.HasValue)
                            {
                                Family.Nom = NameBox.Text;
                            }
                            // Sinon, on crée la famille
                            else
                            {
                                Family = new Familles(Id, NameBox.Text);
                            }

                            Console.WriteLine("Famille : " + Family.Nom + " " + Family.Id);
                            DaoRegistery.GetInstance.DaoFamille.Save(Family);
                            break;
                        }
                    case ActiveList.Subfamily:
                        {
                            // Si l'id a une valeur, on modifie le nom de la sous-famille et la famille
                            if (Id.HasValue)
                            {
                                SubFamily.Nom = NameBox.Text;
                                SubFamily.Famille = (Familles)((ComboBoxItem)FamillyComboBox.SelectedItem).Value;
                            }
                            // Sinon, on crée la sous-famille
                            else
                            {
                                SubFamily = new SousFamilles(Id, (Familles)((ComboBoxItem)FamillyComboBox.SelectedItem).Value, NameBox.Text);
                            }
                            DaoRegistery.GetInstance.DaoSousFamille.Save(SubFamily);
                            break;
                        }
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            // On affiche un message d'erreur s'il y a un champ vide
            else
            {
                MessageBox.Show("Veillez remplir tous les champs", "Error", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Annule la modification ou l'ajout. 
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement</param>
        private void NotValidateButton_Click(object Sender, EventArgs Event)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
