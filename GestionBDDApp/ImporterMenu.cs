using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GestionBDDApp.data;
using GestionBDDApp.data.csv;
using GestionBDDApp.data.model;

namespace GestionBDDApp
{
    public partial class ImporterMenu : Form
    {
        public ImporterMenu()
        {
            InitializeComponent();
        }

        private void AppendModeButton_Click(object sender, EventArgs e)
        {
            string ChoosedFilePath = this.PathChoosedFile.Text;
            if (ChoosedFilePath.Length != 0) {
                try
                {
                    List<ParsingException> annomalies = new List<ParsingException>();
                    List<Articles> toImport = new List<Articles>();
                    ImportProgress.Style = ProgressBarStyle.Marquee;;
                    ImportProgress.MarqueeAnimationSpeed = 30;
                    ImportProgress.Maximum = 100;
                    StatusText.Text = "Lecture du fichier en cours...";
                    CSVReader.ReadFile(ChoosedFilePath, strings => {
                        // Description;Ref;Marque;Famille;Sous-Famille;Prix H.T.
                        try
                        {
                            toImport.Add(Articles.FromRawData(
                                strings[0], strings[1], strings[2], strings[3], strings[4], strings[5]));
                        }
                        catch (ParsingException ParsingException)
                        {
                            annomalies.Add(ParsingException);
                        }

                        Console.WriteLine("Description :" + strings[0] +
                                          "| Ref : " + strings[1] +
                                          "| Marque : " + strings[2] +
                                          "| Famille : " + strings[3] +
                                          "| Sous-Famille : " + strings[4] +
                                          "Prix H.T : " + strings[5]);
                    });

                    StatusText.Text = "Calcul des données à creer...";

                    ImportProgress.Style = ProgressBarStyle.Continuous;
                    ImportProgress.Value = ImportProgress.Minimum = 0;
                    ImportProgress.Maximum = ImportProgress.Minimum;
                    List<Object> dependancies = new List<object>();

                    //calculer les autres tables crées
                    for (var i = 0; i < toImport.Count; i++)
                    {
                        ImportProgress.Value = i;
                    }

                    //si annomalie, demander confirmation
                    if (annomalies.Count == 0)
                    {
                        StatusText.Text = "Résolution des annomalies...";

                    }

                    //import + afficher les résultats
                    ImportProgress.Value = 0;
                    ImportProgress.Maximum = dependancies.Count + toImport.Count;

                    StatusText.Text = "Import des données...";

                    StatusText.Text = "Import terminé";
                }
                catch (Exception Exception)
                {
                    MessageBox.Show(Exception.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Veuillez choisir un fichier", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EreaseModeButton_Click(object sender, EventArgs e)
        {

        }

        private void SelectCsvButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.Title = "Choose a csv file to import";
            OpenFileDialog.DefaultExt = "csv";
            OpenFileDialog.Filter = "csv files (*.csv)|*.csv";
            OpenFileDialog.CheckFileExists = true;
            OpenFileDialog.CheckPathExists = true;
            DialogResult result = OpenFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                StatusText.Text = "Prêt à importer";
                EreaseModeButton.Enabled = true;
                AppendModeButton.Enabled = true;
                PathChoosedFile.Text = OpenFileDialog.FileName;
            }
        }
    }
}
