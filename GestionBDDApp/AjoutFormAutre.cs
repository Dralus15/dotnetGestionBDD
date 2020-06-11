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
        public AjoutFormAutre(ActiveList type) //TODO encapsulation
        {
            InitializeComponent();
            if (type == ActiveList.Subfamily)
            {
                FamillyComboBox.Visible = true;
                label2.Visible = true;
            }
            this.type = type;
            Id = -1;
        }

        public AjoutFormAutre(ActiveList type, int Id)
        {
            InitializeComponent();
            this.type = type;
            this.Id = Id;
            switch (type)
            {
                case ActiveList.Brand:
                    {
                        Brand = DaoRegistery.GetInstance.DaoMarque.GetMarqueById(Id);
                        DescriptionBox.Text = Brand.Nom;
                        IdBox.Value = (decimal) Brand.Id;
                        break;
                    }
                case ActiveList.Family:
                    {
                        Family = DaoRegistery.GetInstance.DaoFamille.GetFamilleById(Id);
                        DescriptionBox.Text = Family.Nom;
                        IdBox.Value = (decimal) Family.Id;
                        break;
                    }
                case ActiveList.Subfamily:
                    SubFamily = DaoRegistery.GetInstance.DaoSousFamille.GetSousFamilleById(Id);
                    DescriptionBox.Text = SubFamily.Nom;
                    IdBox.Value = (decimal) SubFamily.Id;
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

        private int Id;
        private Marques Brand;
        private Familles Family;
        private List<Familles> FamilyModel = new List<Familles>();
        private SousFamilles SubFamily;
        private ActiveList type;
        private void ValidateButton_Click(object sender, EventArgs e)
        {
            if ( DescriptionBox.TextLength > 0 )
            {
                switch (type)
                {
                    case ActiveList.Brand:
                        {
                            if (Id > -1)
                            {
                                Brand.Nom = DescriptionBox.Text;
                                //Brand.Id = (int)IdBox.Value;
                            }
                            else
                            {
                                Brand = new Marques((int)IdBox.Value, DescriptionBox.Text);
                            }
                            Console.WriteLine("Brand : " + Brand.Nom + " " + Brand.Id);
                            DaoRegistery.GetInstance.DaoMarque.Save(Brand);
                            break;
                        }
                    case ActiveList.Family:
                        {
                            if (Id > -1)
                            {
                                Family.Nom = DescriptionBox.Text;
                                //Family.Id = (int) IdBox.Value;
                            }
                            else
                            {
                                Family = new Familles((int)IdBox.Value, DescriptionBox.Text);
                            }
                            Console.WriteLine("Famille : " + Family.Nom + " " + Family.Id);
                            DaoRegistery.GetInstance.DaoFamille.Save(Family);
                            break;
                        }
                    case ActiveList.Subfamily:
                        {
                            if (Id > -1)
                            {
                                SubFamily.Nom = DescriptionBox.Text;
                                //SubFamily.Id = (int) IdBox.Value;
                                SubFamily.Famille = (Familles)((ComboBoxItem)FamillyComboBox.SelectedItem).Value;
                            }
                            else
                            {
                                SubFamily = new SousFamilles((int)IdBox.Value, (Familles)((ComboBoxItem)FamillyComboBox.SelectedItem).Value, DescriptionBox.Text);
                            }
                            Console.WriteLine("Famille : " + SubFamily.Nom + " " + SubFamily.Id + " " + SubFamily.Famille.Nom);
                            DaoRegistery.GetInstance.DaoSousFamille.Save(SubFamily);
                            break;
                        }
                }
                this.Close();
            }
            else
                MessageBox.Show("Veillez remplir tous les champs", "Error", MessageBoxButtons.OK);
        }

        private void NotValidateButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
