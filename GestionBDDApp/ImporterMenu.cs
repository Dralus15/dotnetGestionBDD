using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using GestionBDDApp.data;
using GestionBDDApp.data.csv;
using GestionBDDApp.data.dao;
using GestionBDDApp.data.dto;
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
                    var Annomalies = new Dictionary<int, string>();
                    var ArticlesRef = new Dictionary<string, int>();
                    var ToImport = new List<ArticlesDto>();
                    ImportProgress.Style = ProgressBarStyle.Marquee;;
                    ImportProgress.MarqueeAnimationSpeed = 30;
                    ImportProgress.Maximum = 100;
                    StatusText.Text = "Lecture du fichier en cours...";
                    CSVReader.ReadFile(ChoosedFilePath, (Strings, LineNumber) => {
                        try
                        {
                            var DescStr = Strings[1];
                            int previousLineDefinition;
                            if (ArticlesRef.TryGetValue(DescStr, out previousLineDefinition))
                            {
                                Annomalies.Add(LineNumber, String.Format("Réference d'article en doublon (précédement défini à la ligne {0})", previousLineDefinition));
                            }
                            else
                            {
                                ArticlesRef.Add(DescStr, LineNumber);
                                ToImport.Add(ArticlesDto.FromRawData(
                                    // Description;Ref;Marque;Famille;Sous-Famille;Prix H.T.
                                    Strings[0], DescStr, Strings[2], Strings[3], Strings[4], Strings[5]));
                            }
                        }
                        catch (ParsingException ParsingException)
                        {
                            Annomalies.Add(LineNumber, ParsingException.Message);
                        }
                    });

                    StatusText.Text = "Calcul des données à créer...";

                    ImportProgress.Style = ProgressBarStyle.Continuous;
                    ImportProgress.Maximum = ToImport.Count;
                    ImportProgress.Minimum = 0;
                    ImportProgress.Value = 0;

                    //si annomalie(s), demander confirmation
                    if (Annomalies.Count > 0)
                    {
                        StringBuilder AnnomaliesError = new StringBuilder();
                        foreach (var Annomaly in Annomalies)
                        {
                            AnnomaliesError.Append(String.Format(" - Line {0} : {1}\n", Annomaly.Key, Annomaly.Value));
                        }
                        StatusText.Text = "Résolution des annomalies...";
                        var ConfirmResult =  MessageBox.Show("Il y a " + Annomalies.Count + " annomalies dans le fichier d'import, ces articles ne seront pas importés :\n" + 
                                                             AnnomaliesError + "\n Voulez vous annuler l'opération ?",
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
                        ImportProgress.Maximum = ToImport.Count * 2;
                        StatusText.Text = "Dédoublonnage...";
                        ImportProgress.Value = 0;
                        foreach (var ArticlesDto in ToImport)
                        {
                            ImportProgress.Value += 1;

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
                        }
                        NameSakeErrorBuilder.AppendFormat("{0} doublons de familles ont été détéctés\n", DuplicateFamilyCount);
                        NameSakeErrorBuilder.AppendFormat("{0} doublons de sous-familles ont été détéctés\n", DuplicateSubFamilyCount);
                        NameSakeErrorBuilder.AppendFormat("{0} doublons de marques ont été détéctés\n", DuplicateBrandCount);
                    }

                    if (!ShouldEreaseBase)
                    {
                        var Error = NameSakeErrorBuilder.ToString();
                        if (Error.Length > 0)
                        {
                            var Result = MessageBox.Show(
                                "Des doublons ont été détéctés : \n" + Error +
                                "Si vous continuer, les doublons ne serons pas importés",
                                "Doublons détéctés", MessageBoxButtons.OKCancel);
                            if (Result == DialogResult.Cancel)
                            {
                                StatusText.Text = "Import annulé.";
                                return;
                            }
                        }
                    }
                    
                    var NewArticles = new Dictionary<Articles, Articles>();
                    int ArticleNameSakeCount = 0;

                    //calculer les autres tables crées
                    foreach (var ArticleDto in ToImport)
                    {
                        //barre de chargement
                        ImportProgress.Value += 1;
                        
                        //résolution des dépendances des marques
                        var Marque = CollectionsUtils.GetOrCreate(NewBrands, ArticleDto.Marque, () => new Marques(null, ArticleDto.Marque));
                        
                        //résolution des dépendances des Familles
                        var Famille = CollectionsUtils.GetOrCreate(NewFamilies, ArticleDto.Famille, () => new Familles(null, ArticleDto.Famille));
                        
                        //résolution des dépendances des Sous-Familles
                        var SousFamille = CollectionsUtils.GetOrCreate(NewSubFamilies, ArticleDto.SousFamille,() => new SousFamilles(null, Famille, ArticleDto.SousFamille));

                        var ArticleNameSake = DaoArticle.GetArticleById(ArticleDto.RefArticle);
                        if (ArticleNameSake != null)
                        {
                            ArticleNameSakeCount++;
                        }
                        NewArticles.Add(new Articles(ArticleDto.RefArticle, ArticleDto.Description, SousFamille, Marque, ArticleDto.Prix, 0), ArticleNameSake);
                    }

                    var NamesakeStrategyChoosed = NamesakeStrategy.IGNORE;
                    
                    if (ArticleNameSakeCount > 0)
                    {
                        var Result = MessageBox.Show(
                            String.Format("{0} doublons d'articles ont été détéctés : \n", ArticleNameSakeCount) +
                            "Voulez-vous ignorer ces doublons ? (sinon les valeurs dans la base seront mise à jour)",
                            "Doublons détéctés", MessageBoxButtons.YesNoCancel);
                        switch (Result)
                        {
                            case DialogResult.Cancel:
                                StatusText.Text = "Import annulé.";
                                return;
                            case DialogResult.No:
                                NamesakeStrategyChoosed = NamesakeStrategy.REPLACE;
                                break;
                        }
                    }

                    if (ShouldEreaseBase)
                    {
                        DaoRegistery.GetInstance.ClearAll();
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
                    foreach (var ArticlePair in NewArticles)
                    {
                        ImportProgress.Value++;
                        StatusText.Text = "Import des articles " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                        //pas de doublons
                        if (ArticlePair.Value == null)
                        {
                            await Task.Run(() => DaoArticle.create(ArticlePair.Key));
                        }
                        else
                        {
                            if (NamesakeStrategyChoosed == NamesakeStrategy.REPLACE)
                            {
                                await Task.Run(() => DaoArticle.update(ArticlePair.Key));
                            }
                        }
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

    public enum NamesakeStrategy
    {
        REPLACE,
        IGNORE
    }
}
