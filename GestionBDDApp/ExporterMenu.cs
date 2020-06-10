using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;

namespace GestionBDDApp
{
    public partial class ExporterMenu : Form
    {
        private DAOArticle DaoArticle;

        public ExporterMenu()
        {
            InitializeComponent();
            DaoArticle = DaoRegistery.GetInstance.DaoArticle;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.Title = "Choose where to export your base";
            SaveFileDialog.DefaultExt = "csv";
            SaveFileDialog.Filter = "Directory | directory";
            SaveFileDialog.CheckFileExists = false;
            SaveFileDialog.CheckPathExists = true;
            SaveFileDialog.FileName = "Export.csv";
            DialogResult Result = SaveFileDialog.ShowDialog();
            if (Result == DialogResult.OK)
            {
                FileChoosedBox.Text = SaveFileDialog.FileName;
                ExportCsvButton.Enabled = true;
            }
        }

        private void ExportCsvButton_Click(object sender, EventArgs e)
        {
            string path = FileChoosedBox.Text;
            if (File.Exists(path))
            {
                var ConfirmResult =  MessageBox.Show("Un fichier de ce nom existe déjà à cet emplacement, cette opération va l'écraser, voulez-vous continuer ?",
                    "Confirmation",
                    MessageBoxButtons.YesNo);
                if (ConfirmResult != DialogResult.Yes) return;
            }
            using (StreamWriter Writer = new StreamWriter(FileChoosedBox.Text, false, Encoding.Default))
            {
                Writer.WriteLine("Description;Ref;Marque;Famille;Sous-Famille;Prix H.T."); //TODO barre de chargement
                foreach (var Articles in DaoArticle.getAll())
                {
                    Writer.WriteLine(
                        $"{Articles.Description};{Articles.RefArticle};{Articles.Marque.Nom};{Articles.SousFamille.Famille.Nom};{Articles.SousFamille.Nom};{Articles.Prix}");
                }
            }
            //TODO afficher bravo export terminé
        }
    }
}