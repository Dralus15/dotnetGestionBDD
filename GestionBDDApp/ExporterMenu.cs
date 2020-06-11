using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GestionBDDApp.data.dao;

namespace GestionBDDApp
{
    public partial class ExporterMenu : Form
    {
        private DaoArticle DaoArticle;

        public ExporterMenu()
        {
            InitializeComponent();
            DaoArticle = DaoRegistery.GetInstance.DaoArticle;
        }

        private void BrowseButton_Click(object Sender, EventArgs Event)
        {
            var SaveFileDialog = new SaveFileDialog
            {
                Title = "Choisir l'emplacement de l'export",
                DefaultExt = "csv",
                Filter = "Directory | directory",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Export.csv"
            };
            var Result = SaveFileDialog.ShowDialog();
            if (Result == DialogResult.OK)
            {
                FileChoosedBox.Text = SaveFileDialog.FileName;
                ExportCsvButton.Enabled = true;
            }
        }

        private void ExportCsvButton_Click(object Sender, EventArgs Event)
        {
            var Path = FileChoosedBox.Text;
            if (File.Exists(Path))
            {
                var ConfirmResult =  MessageBox.Show(
                    "Un fichier de ce nom existe déjà à cet emplacement, cette opération va l'écraser, voulez-vous continuer ?",
                    "Confirmation",
                    MessageBoxButtons.YesNo);
                if (ConfirmResult != DialogResult.Yes) return;
            }
            using (var Writer = new StreamWriter(FileChoosedBox.Text, false, Encoding.Default))
            {
                Writer.WriteLine("Description;Ref;Marque;Famille;Sous-Famille;Prix H.T."); //TODO barre de chargement
                foreach (var Articles in DaoArticle.GetAll())
                {
                    Writer.WriteLine(
                        $"{Articles.Description};{Articles.RefArticle};{Articles.Marque.Nom};{Articles.SousFamille.Famille.Nom};{Articles.SousFamille.Nom};{Articles.Prix}");
                }
            }
            MessageBox.Show("Export terminé !", "Information", MessageBoxButtons.OK);
        }
    }
}