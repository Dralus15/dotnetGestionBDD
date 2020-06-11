using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using System;
using System.Text;
using System.Windows.Forms;

namespace GestionBDDApp
{
    /// <summary>
    /// Fenêtre du formulaire d'ajout ou de modification pour une marque, une famille ou une sous-famille. 
    /// </summary>
    public partial class DefaultForm : Form
    {

        /// <summary>
        /// Id de l'objet.
        /// </summary>
        private int? Id;
        /// <summary>
        /// L'objet Marques.
        /// </summary>
        private Marques Brand;
        /// <summary>
        /// L'objet Familles.
        /// </summary>
        private Family Family;
        /// <summary>
        /// L'objet SousFamilles.
        /// </summary>
        private SubFamily SubFamily;
        /// <summary>
        /// Type de l'objet à modifier ou créer.
        /// </summary>
        private readonly ActiveList Type;
        
        /// <summary>
        /// Crée la fenêtre de création d'une marque, d'une famille ou d'une sous-famille.
        /// </summary>
        public DefaultForm(ActiveList Type)
        {
            InitializeComponent();
            if (Type == ActiveList.Subfamily)
            {
                FamillyComboBox.Visible = true;
                NameLabel.Visible = true;
            }
            this.Type = Type;
            Id = null;
            // On Initialise les valeurs des champs du formulaire.
            Initialize("Formulaire de création");
        }

        /// <summary>
        /// Crée la fenêtre de modification d'une marque, d'une famille ou d'une sous-famille.
        /// </summary>
        public DefaultForm(ActiveList Type, int? Id)
        {
            InitializeComponent();
            this.Type = Type;
            this.Id = Id;
            // On Initialise les valeurs des champs du formulaire.
            Initialize("Formulaire de modification");
        }

        /// <summary>
        /// Initialise les valeurs des champs de la fenêtre de modification.
        /// </summary>
        private void Initialize(string Title)
        {
            Text = Title;
            switch (Type)
            {
                case ActiveList.Brand:
                {
                    // Si l'id a une valeur, on récupère la marque correspondante.
                    if (Id.HasValue)
                    {
                        Brand = DaoRegistry.GetInstance.BrandDao.GetMarqueById(Id.Value);
                        NameBox.Text = Brand.Nom;
                    }

                    break;
                }
                case ActiveList.Family:
                {
                    // Si l'id a une valeur, on récupère la famille correspondante.
                    if (Id.HasValue)
                    {
                        Family = DaoRegistry.GetInstance.FamilyDao.GetFamilleById(Id.Value);
                        NameBox.Text = Family.Name;
                    }

                    break;
                }
                case ActiveList.Subfamily:
                {
                    // On récupère toutes les familles pour les metttre dans la ComboBox des familles.
                    foreach (var AFamily in DaoRegistry.GetInstance.FamilyDao.GetAllFamilles())
                    {
                        FamillyComboBox.Items.Add(new ComboBoxItem(AFamily.Name, AFamily));
                    }
                    // Si l'id a une valeur, on récupère la sous-famille correspondante.
                    if (Id.HasValue)
                    {
                        SubFamily = DaoRegistry.GetInstance.SubFamilyDao.GetSousFamilleById(Id.Value);
                        NameBox.Text = SubFamily.Name;
                        FamillyComboBox.Text = SubFamily.Family.Name;
                    }
                    // On affiche la ComboBox des familles.
                    FamillyComboBox.Visible = true;
                    NameLabel.Visible = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Valide le formulaire s'il est rempli (affiche un message d'erreur sinon) pour créer ou modifier un objet. 
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement.</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement.</param>
        private void ValidateButton_Click(object Sender, EventArgs Event)
        {
            // On vérifie que le champs de description est rempli.
            var ErrorBuilder = new StringBuilder();
            var Description = NameBox.Text.Trim();
            if (Description.Length > 50 || Description.Length < 1)
            {
                ErrorBuilder.AppendLine(
                    "La description doit faire entre 1 et 150 caractères (espace avant et après non-inclus)");
            }
            var Error = ErrorBuilder.ToString();
            if ( Error.Length == 0 )
            {
                switch (Type)
                {
                    case ActiveList.Brand:
                    {
                        // Si l'id a une valeur, on modifie le nom de la marque.
                        if (Id.HasValue)
                        {
                            Brand.Nom = NameBox.Text;
                        }
                        // Sinon, on crée la marque.
                        else
                        {
                            Brand = new Marques(null, NameBox.Text);
                        }
                        DaoRegistry.GetInstance.BrandDao.Save(Brand);
                        break;
                    }
                    case ActiveList.Family:
                    {
                        // Si l'id a une valeur, on modifie le nom de la famille.
                        if (Id.HasValue)
                        {
                            Family.Name = NameBox.Text;
                        }
                        // Sinon, on crée la famille.
                        else
                        {
                            Family = new Family(Id, NameBox.Text);
                        }

                        DaoRegistry.GetInstance.FamilyDao.Save(Family);
                        break;
                    }
                    case ActiveList.Subfamily:
                    {
                        // Si l'id a une valeur, on modifie le nom de la sous-famille et la famille.
                        if (Id.HasValue)
                        {
                            SubFamily.Name = NameBox.Text;
                            SubFamily.Family = (Family)((ComboBoxItem)FamillyComboBox.SelectedItem).Value;
                        }
                        // Sinon, on crée la sous-famille.
                        else
                        {
                            SubFamily = new SubFamily(Id, (Family)((ComboBoxItem)FamillyComboBox.SelectedItem).Value,
                            NameBox.Text);
                        }
                        DaoRegistry.GetInstance.SubFamilyDao.Save(SubFamily);
                        break;
                    }
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            // On affiche un message d'erreur s'il y a au moins un champ vide.
            else
            {
                MessageBox.Show("Les champs suivants sont en erreur dans le formulaire : \n\n" + Error + 
                                "\nCorriger ces erreurs avant de sauvegarder.", "Error", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Annule la modification ou l'ajout. 
        /// </summary>
        /// <param name="Sender"><b>Object</b> est l'objet qui a causé l'événement.</param>
        /// <param name="Event"><b>EventArgs</b> contient l'événement.</param>
        private void NotValidateButton_Click(object Sender, EventArgs Event)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
