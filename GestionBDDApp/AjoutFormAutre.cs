using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using System;
using System.Windows.Forms;

namespace GestionBDDApp
{
    public partial class AjoutFormAutre : Form
    {

        private int? Id;
        private Marques Brand;
        private Familles Family;
        private SousFamilles SubFamily;
        private ActiveList Type;
        
        public AjoutFormAutre(ActiveList Type) //TODO encapsulation
        {
            InitializeComponent();
            Text = "Formulaire de création";
            if (Type == ActiveList.Subfamily)
            {
                FamillyComboBox.Visible = true;
                label2.Visible = true;
            }
            this.Type = Type;
            Id = null;
            Initialize();
        }

        public AjoutFormAutre(ActiveList Type, int? Id)
        {
            InitializeComponent();
            Text = "Formulaire de modification";
            this.Type = Type;
            this.Id = Id;
            Initialize();
        }

        private void Initialize()
        {
            switch (Type)
            {
                case ActiveList.Brand:
                {
                    if (Id.HasValue)
                    {
                        Brand = DaoRegistery.GetInstance.DaoMarque.GetMarqueById(Id.Value);
                        DescriptionBox.Text = Brand.Nom;
                    }

                    break;
                }
                case ActiveList.Family:
                {
                    if (Id.HasValue)
                    {
                        Family = DaoRegistery.GetInstance.DaoFamille.GetFamilleById(Id.Value);
                        DescriptionBox.Text = Family.Nom;
                    }

                    break;
                }
                case ActiveList.Subfamily:
                {
                    foreach (var Family in DaoRegistery.GetInstance.DaoFamille.GetAllFamilles())
                    {
                        FamillyComboBox.Items.Add(new ComboBoxItem(Family.Nom, Family));
                    }

                    if (Id.HasValue)
                    {
                        SubFamily = DaoRegistery.GetInstance.DaoSousFamille.GetSousFamilleById(Id.Value);
                        DescriptionBox.Text = SubFamily.Nom;
                        FamillyComboBox.Text = SubFamily.Famille.Nom;
                    }

                    FamillyComboBox.Visible = true;
                    label2.Visible = true;
                    break;
                }
            }
        }

        private void ValidateButton_Click(object Sender, EventArgs Event)
        {
            if ( DescriptionBox.TextLength > 0 )
            {
                switch (Type)
                {
                    case ActiveList.Brand:
                        {
                            if (Id.HasValue)
                            {
                                Brand.Nom = DescriptionBox.Text;
                            }
                            else
                            {
                                Brand = new Marques(null, DescriptionBox.Text);
                            }
                            DaoRegistery.GetInstance.DaoMarque.Save(Brand);
                            break;
                        }
                    case ActiveList.Family:
                        {
                            if (Id.HasValue)
                            {
                                Family.Nom = DescriptionBox.Text;
                            }
                            else
                            {
                                Family = new Familles(Id, DescriptionBox.Text);
                            }

                            Console.WriteLine("Famille : " + Family.Nom + " " + Family.Id);
                            DaoRegistery.GetInstance.DaoFamille.Save(Family);
                            break;
                        }
                    case ActiveList.Subfamily:
                        {
                            if (Id.HasValue)
                            {
                                SubFamily.Nom = DescriptionBox.Text;
                                SubFamily.Famille = (Familles)((ComboBoxItem)FamillyComboBox.SelectedItem).Value;
                            }
                            else
                            {
                                SubFamily = new SousFamilles(Id, (Familles)((ComboBoxItem)FamillyComboBox.SelectedItem).Value, DescriptionBox.Text);
                            }
                            DaoRegistery.GetInstance.DaoSousFamille.Save(SubFamily);
                            break;
                        }
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Veillez remplir tous les champs", "Error", MessageBoxButtons.OK);
            }
        }

        private void NotValidateButton_Click(object Sender, EventArgs Event)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
