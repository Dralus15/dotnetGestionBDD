using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionBDDApp
{
    public partial class AjoutFormAutre : Form
    {

        private int? Id;
        private Marques Brand;
        private Familles Family;
        private List<Familles> FamilyModel = new List<Familles>();
        private SousFamilles SubFamily;
        private ActiveList type;
        public AjoutFormAutre(ActiveList Type) //TODO encapsulation
        {
            InitializeComponent();
            if (Type == ActiveList.Subfamily)
            {
                FamillyComboBox.Visible = true;
                label2.Visible = true;
            }
            type = Type;
            Id = null;
        }

        public AjoutFormAutre(ActiveList Type, int? Id)
        {
            InitializeComponent();
            type = Type;
            this.Id = Id;
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
                    if (Id.HasValue)
                    {
                        SubFamily = DaoRegistery.GetInstance.DaoSousFamille.GetSousFamilleById(Id.Value);
                        DescriptionBox.Text = SubFamily.Nom;
                    }
                    FamillyComboBox.Visible = true;
                    label2.Visible = true;
                    FamilyModel = DaoRegistery.GetInstance.DaoFamille.GetAllFamilles();
                    foreach (var Family in FamilyModel)
                    {
                        FamillyComboBox.Items.Add(new ComboBoxItem(Family.Nom, Family));
                    }
                    break;
            }
        }
        private void ValidateButton_Click(object Sender, EventArgs Event)
        {
            if ( DescriptionBox.TextLength > 0 )
            {
                switch (type)
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
                Close();
            }
            else
            {
                MessageBox.Show("Veillez remplir tous les champs", "Error", MessageBoxButtons.OK);
            }
        }

        private void NotValidateButton_Click(object Sender, EventArgs Event)
        {
            Close();
        }
    }
}
