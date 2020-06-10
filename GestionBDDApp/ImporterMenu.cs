using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GestionBDDApp.data;
using GestionBDDApp.data.csv;
using GestionBDDApp.data.dao;
using GestionBDDApp.data.model;
using GestionBDDApp.data.utils;


namespace GestionBDDApp
{
    public partial class ImporterMenu : Form
    {
        private DAOArticle DaoArticle;
        private DAOFamille DaoFamille;
        private DAOMarque DaoMarque;
        private DAOSousFamille DaoSousFamille;

        public ImporterMenu()
        {
            InitializeComponent();
            DaoArticle = DaoRegistery.GetInstance.DaoArticle;
            DaoFamille = DaoRegistery.GetInstance.DaoFamille;
            DaoMarque = DaoRegistery.GetInstance.DaoMarque;
            DaoSousFamille = DaoRegistery.GetInstance.DaoSousFamille;
        }

        private void AppendModeButton_Click(object sender, EventArgs e)
        {
            Import(false);
        }

        private void EreaseModeButton_Click(object sender, EventArgs e)
        {
            var ConfirmResult =  MessageBox.Show("Cette action va écraser la base, êtes-vous sur de continuer ?",
                "Confirmation",
                MessageBoxButtons.YesNo);
            if (ConfirmResult == DialogResult.Yes)
            {
                DaoRegistery.GetInstance.ClearAll();
                Import(true);
            }
        }

        private void Import(bool BaseIsEmpty)
        {
            int Id = 0;
            string ChoosedFilePath = this.PathChoosedFile.Text;
            if (ChoosedFilePath.Length != 0) {
                try
                {
                    var Annomalies = new Dictionary<int, ParsingException>();
                    var ToImport = new List<ArticlesDto>();
                    ImportProgress.Style = ProgressBarStyle.Marquee;;
                    ImportProgress.MarqueeAnimationSpeed = 30;
                    ImportProgress.Maximum = 100;
                    StatusText.Text = "Lecture du fichier en cours...";
                    CSVReader.ReadFile(ChoosedFilePath, (Strings, LineNumber) => {
                        // Description;Ref;Marque;Famille;Sous-Famille;Prix H.T.
                        try
                        {
                            ToImport.Add(Articles.FromRawData(
                                Strings[0], Strings[1], Strings[2], Strings[3], Strings[4], Strings[5]));
                        }
                        catch (ParsingException ParsingException)
                        {
                            Annomalies.Add(LineNumber, ParsingException);
                        }
                        
                        Console.WriteLine("Description :" + Strings[0] +
                                          "| Ref : " + Strings[1] +
                                          "| Marque : " + Strings[2] +
                                          "| Famille : " + Strings[3] +
                                          "| Sous-Famille : " + Strings[4] +
                                          "| Prix H.T : " + Strings[5]);
                    });

                    StatusText.Text = "Calcul des données à creer...";

                    ImportProgress.Style = ProgressBarStyle.Continuous;
                    ImportProgress.Maximum = ToImport.Count;
                    ImportProgress.Minimum = 0;
                    ImportProgress.Value = 0;
                    var NewMarques = new Dictionary<string, Marques>();
                    var NewFamilles = new Dictionary<string, Familles>();
                    var NewSousFamilles = new Dictionary<string, SousFamilles>();
                    
                    var NewArticles = new List<Articles>();

                    //calculer les autres tables crées
                    for (var ToImportIndex = 0; ToImportIndex < ToImport.Count; ToImportIndex++)
                    {
                        //barre de chargement
                        ImportProgress.Value = ToImportIndex + 1;

                        var ArticleDto = ToImport[ToImportIndex];
                        

                        Marques Marque = null;
                        Familles Famille = null;
                        SousFamilles SousFamille = null;
                        
                        var MarqueName = ArticleDto.Marque;
                        var FamilleName = ArticleDto.Famille;
                        var SousFamilleName = ArticleDto.SousFamille;
                        
                        if (! BaseIsEmpty)
                        {
                            //TODO gerer les modifications des familles
                            // Famille = DaoFamille.GetFamilleByName(FamilleName);
                            // Marque = DaoMarque.GetMarqueByName(MarqueName);
                            // SousFamille = DaoSousFamille.getSousFamilleByName(SousFamilleName);
                        }
                        
                        //résolution des dépendances des marques
                        if (Marque == null) {
                            Marque = CollectionsUtils.GetOrCreate(NewMarques, MarqueName, () => new Marques(null, MarqueName));
                        }
                        
                        //résolution des dépendances des Familles
                        if (Famille == null)
                        {
                            Famille = CollectionsUtils.GetOrCreate(NewFamilles, FamilleName, () => new Familles(null, FamilleName));
                        }
                        
                        //résolution des dépendances des Sous-Familles
                        if (SousFamille == null)
                        {
                            SousFamille = CollectionsUtils.GetOrCreate(NewSousFamilles, SousFamilleName,
                                () => new SousFamilles(null, Famille, SousFamilleName));
                        }

                        NewArticles.Add(new Articles(ArticleDto.RefArticle, ArticleDto.Description, SousFamille, Marque, ArticleDto.Prix, 0));
                    }

                    //si annomalie(s), demander confirmation
                    if (Annomalies.Count > 0)
                    {
                        StringBuilder Sb = new StringBuilder();
                        foreach (var Annomaly in Annomalies)
                        {
                            Sb.Append(String.Format(" - Line {0} : {1}\n", Annomaly.Key, Annomaly.Value.Message));
                        }
                        StatusText.Text = "Résolution des annomalies...";
                        var ConfirmResult =  MessageBox.Show("Il y a " + Annomalies.Count + " annomalies dans le fichier d'import, ces articles ne seront pas importés :\n" + 
                                                             Sb + "\n Voulez vous annuler l'opération ?",
                            "Annomalies détéctées",
                            MessageBoxButtons.OKCancel);
                        if (ConfirmResult == DialogResult.Cancel)
                        {
                            StatusText.Text = "Import annulé";
                            return;
                        }
                    }
                    StatusText.Text = "Import des données...";

                    ImportProgress.Value = 0;
                    ImportProgress.Maximum = NewMarques.Count;
                    foreach (var Marques in NewMarques.Values)
                    {
                        ImportProgress.Value++;
                        StatusText.Text = "Import des marques " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                        DaoMarque.save(Marques);
                    }
                    
                    ImportProgress.Value = 0;
                    ImportProgress.Maximum = NewFamilles.Count;
                    foreach (var Familles in NewFamilles.Values)
                    {
                        ImportProgress.Value++;
                        StatusText.Text = "Import des familles " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                        DaoFamille.save(Familles);
                    }
                    
                    ImportProgress.Value = 0;
                    ImportProgress.Maximum = NewSousFamilles.Count;
                    foreach (var SousFamille in NewSousFamilles.Values)
                    {
                        ImportProgress.Value++;
                        StatusText.Text = "Import des sous-familles " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                        DaoSousFamille.save(SousFamille);
                    }
                    
                    ImportProgress.Value = 0;
                    ImportProgress.Maximum = NewArticles.Count;
                    foreach (var Article in NewArticles)
                    {
                        ImportProgress.Value++;
                        StatusText.Text = "Import des articles " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                        DaoArticle.save(Article);
                    }
                    
                    StatusText.Text = "Import terminé";
                    MessageBox.Show("Import terminé :\n" + 
                            NewMarques.Values.Count + " nouvelles marques \n" +
                            NewFamilles.Values.Count + " nouvelles familles \n" +
                            NewSousFamilles.Values.Count + " nouvelles sous-familles\n" +
                            NewArticles.Count + " nouveaux articles \n",
                        "Import terminé avec succés.",
                        MessageBoxButtons.OK);
                }
                catch (Exception Exception)
                {
                    Console.WriteLine(Exception.ToString());
                    MessageBox.Show(Exception.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Veuillez choisir un fichier", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectCsvButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.Title = "Choose a csv file to import";
            OpenFileDialog.DefaultExt = "csv";
            OpenFileDialog.Filter = "csv files (*.csv)|*.csv";
            OpenFileDialog.CheckFileExists = true;
            OpenFileDialog.CheckPathExists = true;
            DialogResult Result = OpenFileDialog.ShowDialog();
            if (Result == DialogResult.OK)
            {
                StatusText.Text = "Prêt à importer";
                EreaseModeButton.Enabled = true;
                AppendModeButton.Enabled = true;
                PathChoosedFile.Text = OpenFileDialog.FileName;
            }
        }
    }
}
