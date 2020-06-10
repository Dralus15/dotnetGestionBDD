using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
                Import(true);
            }
        }

        private async void Import(bool ShouldEreaseBase)
        {
            SetBusy(false);
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
                    
                    var NewBrands = new Dictionary<string, Marques>();
                    var NewFamilies = new Dictionary<string, Familles>();
                    var NewSubFamilies = new Dictionary<string, SousFamilles>();
                    
                    int DuplicateFamilyCount = 0, DuplicateBrandCount = 0, DuplicateSubFamilyCount = 0;

                    //namesake
                    StringBuilder NameSakeErrorBuilder = new StringBuilder();
                    if (! ShouldEreaseBase)
                    {
                        foreach (var ArticlesDto in ToImport)
                        {

                            var FamilyName = ArticlesDto.Famille;
                            if (! NewFamilies.ContainsKey(FamilyName))
                            {
                                var FamilyNameSake = DaoFamille.GetFamilleByName(FamilyName);
                                if (FamilyNameSake.Count > 0)
                                {
                                    DuplicateFamilyCount += FamilyNameSake.Count;
                                    NewFamilies.Add(FamilyName, FamilyNameSake[0]);
                                }
                            }
                            
                            var BrandName = ArticlesDto.Marque;
                            if (! NewBrands.ContainsKey(BrandName))
                            {
                                var BrandNameSake = DaoMarque.GetBrandByName(BrandName);
                                if (BrandNameSake.Count > 0)
                                {
                                    DuplicateBrandCount += BrandNameSake.Count;
                                    NewBrands.Add(BrandName, BrandNameSake[0]);
                                }
                            }
                            
                            var SubFamilyName = ArticlesDto.Famille;
                            if (! NewSubFamilies.ContainsKey(SubFamilyName))
                            {
                                var SubFamilyNameSake = DaoSousFamille.GetSubFamiliesByName(SubFamilyName);
                                if (SubFamilyNameSake.Count > 0)
                                {
                                    DuplicateSubFamilyCount += SubFamilyNameSake.Count;
                                    NewSubFamilies.Add(SubFamilyName, SubFamilyNameSake[0]);
                                }
                            }
                            //TODO tester les modifications des familles
                        }
                        NameSakeErrorBuilder.AppendFormat("{0} doublons de familles ont été détéctés", DuplicateFamilyCount);
                        NameSakeErrorBuilder.AppendFormat("{0} doublons de sous-familles ont été détéctés", DuplicateSubFamilyCount);
                        NameSakeErrorBuilder.AppendFormat("{0} doublons de marques ont été détéctés", DuplicateBrandCount);
                    }
                    
                    var NewArticles = new List<Articles>();

                    //calculer les autres tables crées
                    for (var ToImportIndex = 0; ToImportIndex < ToImport.Count; ToImportIndex++)
                    {
                        //barre de chargement
                        ImportProgress.Value = ToImportIndex + 1;

                        var ArticleDto = ToImport[ToImportIndex];

                        var MarqueName = ArticleDto.Marque;
                        var FamilleName = ArticleDto.Famille;
                        var SousFamilleName = ArticleDto.SousFamille;
                        
                        //TODO dedoublonnage d'article
                        //TODO modif si doublons

                        //résolution des dépendances des marques
                        var Marque = CollectionsUtils.GetOrCreate(NewBrands, MarqueName, () => new Marques(null, MarqueName));
                        
                        //résolution des dépendances des Familles
                        var Famille = CollectionsUtils.GetOrCreate(NewFamilies, FamilleName, () => new Familles(null, FamilleName));
                        
                        //résolution des dépendances des Sous-Familles
                        var SousFamille = CollectionsUtils.GetOrCreate(NewSubFamilies, SousFamilleName,() => new SousFamilles(null, Famille, SousFamilleName));

                        NewArticles.Add(new Articles(ArticleDto.RefArticle, ArticleDto.Description, SousFamille, Marque, ArticleDto.Prix, 0));
                    }

                    if (ShouldEreaseBase)
                    {
                        DaoRegistery.GetInstance.ClearAll();
                    }
                    else
                    {
                        if (NameSakeErrorBuilder.Length > 0)
                        {
                            var Result = MessageBox.Show(
                                "Des doublons ont été détéctés : \n" + NameSakeErrorBuilder.ToString() + "\n" +
                                "Voulez-vous concerver les doublons ? (les nouvelles valeurs ne seront pas importées)",
                                "Doublons détéctés", MessageBoxButtons.YesNoCancel);
                            if (Result == DialogResult.Cancel)
                            {
                                StatusText.Text = "Import annulé.";
                                return;
                            }
                        }
                    }

                    StatusText.Text = "Import des données...";

                    ImportProgress.Value = 0;
                    ImportProgress.Maximum = NewBrands.Count;
                    foreach (var Marques in NewBrands.Values)
                    {
                        ImportProgress.Value++;
                        StatusText.Text = "Import des marques " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                        await Task.Run(() => DaoMarque.save(Marques));
                    }
                    
                    ImportProgress.Value = 0;
                    ImportProgress.Maximum = NewFamilies.Count;
                    foreach (var Familles in NewFamilies.Values)
                    {
                        ImportProgress.Value++;
                        StatusText.Text = "Import des familles " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                        await Task.Run(() => DaoFamille.save(Familles));
                    }
                    
                    ImportProgress.Value = 0;
                    ImportProgress.Maximum = NewSubFamilies.Count;
                    foreach (var SousFamille in NewSubFamilies.Values)
                    {
                        ImportProgress.Value++;
                        StatusText.Text = "Import des sous-familles " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                        await Task.Run(() => DaoSousFamille.save(SousFamille));
                    }
                    
                    ImportProgress.Value = 0;
                    ImportProgress.Maximum = NewArticles.Count;
                    foreach (var Article in NewArticles)
                    {
                        ImportProgress.Value++;
                        StatusText.Text = "Import des articles " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                        await Task.Run(() => DaoArticle.create(Article));
                    }
                    
                    StatusText.Text = "Import terminé";
                    MessageBox.Show("Import terminé :\n" + 
                            NewBrands.Values.Count + " nouvelles marques \n" +
                            NewFamilies.Values.Count + " nouvelles familles \n" +
                            NewSubFamilies.Values.Count + " nouvelles sous-familles\n" +
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
            SetBusy(true);
        }

        private void SetBusy(bool IsNotBusy)
        {
            ControlBox = IsNotBusy;
            AppendModeButton.Enabled = IsNotBusy;
            EreaseModeButton.Enabled = IsNotBusy;
            SelectCsvButton.Enabled = IsNotBusy;
            Application.UseWaitCursor = ! IsNotBusy;
        }

        private void SelectCsvButton_Click(object Sender, EventArgs Event)
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
