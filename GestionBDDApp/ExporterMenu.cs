﻿using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GestionBDDApp.data.dao;

namespace GestionBDDApp
{
    /// <summary>
    /// Fenêtre pour exporter la base vers un fichier.
    /// </summary>
    public partial class ExporterMenu : Form
    {
        /// <summary>
        /// Le Dao des articles pour récupérer les données de la base à exporter.
        /// </summary>
        private readonly ArticleDao ArticleDao;

        /// <summary>
        /// Crée la fenêtre pour exporter le menu.
        /// </summary>
        public ExporterMenu()
        {
            InitializeComponent();
            ArticleDao = DaoRegistry.GetInstance.ArticleDao;
        }

        /// <summary>
        /// Ouvre la fenêtre de dialog pour choisir l'emplacement du fichier.
        /// </summary>
        /// <param name="Sender">Non utilisé.</param>
        /// <param name="Event">Les données de l'événement de clique.</param>
        private void BrowseButton_Click(object Sender, EventArgs Event)
        {
            // Ouvre une fenêtre de sauvegarde de fichier, elle permet de choisir un répertoire et un nom pour le fichier.
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

            // Si l'utilisateur confirme son choix, on sauvegarde le chemin.
            if (Result == DialogResult.OK)
            {
                FileChoosedBox.Text = SaveFileDialog.FileName;
                ExportCsvButton.Enabled = true;
            }
        }

        /// <summary>
        /// Confirme l'export de la base.
        /// Si le fichier est déjà existant, on demande la confirmation avant de l'écraser.
        /// </summary>
        /// <param name="Sender">Non utilisé.</param>
        /// <param name="Event">Les données de l'événement de clique.</param>
        private void ExportCsvButton_Click(object Sender, EventArgs Event)
        {
            var Path = FileChoosedBox.Text;
            // Si le fichier existe, on demande la confirmation à l'utilisateur avant d'éventuellement l'écraser.
            if (File.Exists(Path))
            {
                var ConfirmResult =  MessageBox.Show(
                    "Un fichier de ce nom existe déjà à cet emplacement, cette opération va l'écraser, " +
                    "voulez-vous continuer ?", "Confirmation",
                    MessageBoxButtons.YesNo);
                // Si l'utilisateur ne veut pas écraser le fichier existant, on annule.
                if (ConfirmResult != DialogResult.Yes) return;
            }
            // On affiche la progression de l'export.
            using (var Writer = new StreamWriter(FileChoosedBox.Text, false, Encoding.Default))
            {
                Writer.WriteLine("Description;Ref;Marque;Famille;Sous-Famille;Prix H.T."); //TODO barre de chargement
                var All = ArticleDao.GetAll();
                for (var Index = 0; Index < All.Count; Index++)
                {
                    var Articles = All[Index];
                    Writer.WriteLine(
                        $"{Articles.Description};{Articles.RefArticle};{Articles.Marque.Nom};" +
                        $"{Articles.SubFamily.Family.Name};{Articles.SubFamily.Name};{Articles.Price}");
                }
            }
            // On confirme à l'utilisateur que l'export est terminé.
            MessageBox.Show("Export terminé !", "Information", MessageBoxButtons.OK);
        }
    }
}