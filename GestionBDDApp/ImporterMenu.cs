using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GestionBDDApp.data;
using GestionBDDApp.data.csv;
using GestionBDDApp.data.dao;
using GestionBDDApp.data.dto;
using GestionBDDApp.data.model;
using GestionBDDApp.data.utils;

namespace GestionBDDApp
{
    /// <summary>
    /// Fenetre d'importation de base à partir d'un fichier au format CSV
    /// Elle gère 2 mode d'import :
    ///  - Ajout : ajoute les nouvelles entrée aux donnée existante
    ///  - Ecrasement : supprime la base et importe les données
    ///
    /// Elle gère aussi le dédoublonnage dans le fichier d'import, et avec les données existante (en mode ajout)
    /// </summary>
    public partial class ImporterMenu : Form
    {
        /// <summary>
        /// Un dao pour accéder aux articles
        /// </summary>
        private DaoArticle DaoArticle;
        
        /// <summary>
        /// Un dao pour accéder aux familles
        /// </summary>
        private DaoFamille DaoFamille;
        
        /// <summary>
        /// Un dao pour accéder aux marques
        /// </summary>
        private DaoMarque DaoMarque;
        
        /// <summary>
        /// Un dao pour accéder aux sous-familles
        /// </summary>
        private DaoSousFamille DaoSousFamille;

        /// <summary>
        /// Instancie un nouveau menu d'import
        /// </summary>
        public ImporterMenu()
        {
            InitializeComponent();
            DaoArticle = DaoRegistery.GetInstance.DaoArticle;
            DaoFamille = DaoRegistery.GetInstance.DaoFamille;
            DaoMarque = DaoRegistery.GetInstance.DaoMarque;
            DaoSousFamille = DaoRegistery.GetInstance.DaoSousFamille;
        }

        /// <summary>
        /// Lance l'import
        /// </summary>
        /// <param name="ShouldEraseBase"><c>true</c> si la base doit être supprimé avant l'import</param>
        private async void Import(bool ShouldEraseBase)
        {
            SetBusy(true);
            var ChoosedFilePath = PathChoosedFile.Text;
            if (ChoosedFilePath.Length != 0) {
                try
                {
                    var Annomalies = new Dictionary<int, string>();
                    var ExistingArticlesCountByRef = new Dictionary<string, int>();
                    var ArticlesDtosRead = new List<ArticlesDto>();
                    
                    //On ne donne pas d'étape de chargement pour la lecture du fichier
                    ImportProgress.Style = ProgressBarStyle.Marquee;
                    ImportProgress.MarqueeAnimationSpeed = 30;
                    ImportProgress.Maximum = 100;
                    StatusText.Text = "Lecture du fichier en cours...";
                    ReadFile(ChoosedFilePath, ExistingArticlesCountByRef, Annomalies, ArticlesDtosRead);
                    
                    //Si des annomalies sont présentes dans le fichier on demande une confirmation
                    if (Annomalies.Count > 0)
                    {
                        var AnnomaliesError = new StringBuilder();
                        foreach (var Annomaly in Annomalies)
                        {
                            AnnomaliesError.Append(String.Format(" - Line {0} : {1}\n", Annomaly.Key, Annomaly.Value));
                        }
                        StatusText.Text = "Résolution des annomalies...";
                        var ConfirmResult =  MessageBox.Show("Il y a " + Annomalies.Count + 
                             " annomalies dans le fichier d'import, ces articles ne seront pas importés :\n" + 
                             AnnomaliesError + "\n Voulez vous annuler l'opération ?",
                            "Annomalies détéctées",
                            MessageBoxButtons.OKCancel);
                        if (ConfirmResult == DialogResult.Cancel)
                        {
                            StatusText.Text = "Import annulé";
                            DialogResult = DialogResult.Cancel;
                            return;
                        }
                    }

                    //On remet la barre de chargement à 0 en mode pas à pas
                    StatusText.Text = "Calcul des données à créer...";
                    ImportProgress.Style = ProgressBarStyle.Continuous;
                    ImportProgress.Maximum = ArticlesDtosRead.Count;
                    ImportProgress.Minimum = 0;
                    ImportProgress.Value = 0;

                    //Les données à sauvegarder
                    var NewBrands = new Dictionary<string, Marques>();
                    var NewFamilies = new Dictionary<string, Familles>();
                    var NewSubFamilies = new Dictionary<string, SousFamilles>();
                    
                    //les articles à sauvegarder sous la forme de couple <Nouvelle donnée, doublon>
                    var NewArticles = new Dictionary<Articles, Articles>();
                    
                    //dédoublonnage étape 1 : doublon de familles / sous-familles et marques
                    if (! ShouldEraseBase)
                    {    
                        //comme la base n'a pas été encore touché, on peut encore annuler
                        if (FindDuplicates(ArticlesDtosRead, NewFamilies, NewBrands, NewSubFamilies)) return;
                    }
                    
                    var ArticleNameSakeCount = 0;

                    //calculer les autres tables crées
                    foreach (var ArticleDto in ArticlesDtosRead)
                    {
                        //barre de chargement
                        ImportProgress.Value += 1;
                        
                        //résolution des dépendances des marques
                        var Marque = CollectionsUtils.GetOrCreate(NewBrands, ArticleDto.BrandName, 
                            () => new Marques(null, ArticleDto.BrandName));
                        
                        //résolution des dépendances des Familles
                        var Famille = CollectionsUtils.GetOrCreate(NewFamilies, ArticleDto.FamilyName, 
                            () => new Familles(null, ArticleDto.FamilyName));
                        
                        //résolution des dépendances des Sous-Familles
                        var SousFamille = CollectionsUtils.GetOrCreate(NewSubFamilies, ArticleDto.SubFamilyName,
                            () => new SousFamilles(null, Famille, ArticleDto.SubFamilyName));

                        Articles ArticleNameSake;
                        if (ShouldEraseBase)
                        {
                            ArticleNameSake = null;
                        }
                        else
                        {
                            ArticleNameSake = DaoArticle.GetArticleById(ArticleDto.ArticleRef);
                        }
                        if (ArticleNameSake != null)
                        {
                            ArticleNameSakeCount++;
                        }
                        NewArticles.Add(new Articles(ArticleDto.ArticleRef, ArticleDto.Description, SousFamille, Marque, 
                            ArticleDto.Price, 0), ArticleNameSake);
                    }

                    var NamesakeStrategyChoosed = NamesakeStrategy.Ignore;
                    
                    if (ShouldEraseBase)
                    {
                        DaoRegistery.GetInstance.ClearAll();
                    }
                    else
                    {
                        //Si il y a des doublons
                        if (ArticleNameSakeCount > 0)
                        {
                            //on choisie la stratégie de dédoublonnage, comme la base n'a pas été encore touché,
                            //c'est le dernier moment pour annuler
                            if (ChooseNameSakeStategy(ArticleNameSakeCount, ref NamesakeStrategyChoosed)) return;
                        }
                    }

                    //On enregistre les données
                    await SaveImportedData(NewBrands, NewFamilies, NewSubFamilies, NewArticles, NamesakeStrategyChoosed);

                    StatusText.Text = "Import terminé";
                    
                    //On affiche le rapport d'import
                    MessageBox.Show("Import terminé :\n" + 
                            NewBrands.Values.Count + " nouvelles marques \n" +
                            NewFamilies.Values.Count + " nouvelles familles \n" +
                            NewSubFamilies.Values.Count + " nouvelles sous-familles\n" +
                            NewArticles.Count + " nouveaux articles \n",
                        "Import terminé avec succés.",
                        MessageBoxButtons.OK);
                    DialogResult = DialogResult.OK;
                    Close();
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
            SetBusy(false);
        }

        /// <summary>
        /// Demande à l'utilisateur la façon de gérer les doublons d'articles
        /// </summary>
        /// <param name="ArticleNameSakeCount">Le nombre d'articles en doublons</param>
        /// <param name="NamesakeStrategyChoosed">La stratégie choisie</param>
        /// <returns><c>true</c> si l'import est annulé, <c>false</c> sinon</returns>
        private bool ChooseNameSakeStategy(int ArticleNameSakeCount, ref NamesakeStrategy NamesakeStrategyChoosed)
        {
            var Result = MessageBox.Show(
                String.Format("{0} doublons d'articles ont été détéctés : \n", ArticleNameSakeCount) +
                "Voulez-vous ignorer ces doublons ? (sinon les valeurs dans la base seront mise à jour)",
                "Doublons détéctés", MessageBoxButtons.YesNoCancel);
            switch (Result)
            {
                case DialogResult.Cancel:
                    StatusText.Text = "Import annulé.";
                    DialogResult = DialogResult.Cancel;
                    return true;
                case DialogResult.No:
                    NamesakeStrategyChoosed = NamesakeStrategy.Replace;
                    break;
            }

            return false;
        }

        /// <summary>
        /// Enregistre les nouvelles données en bases
        /// </summary>
        /// <param name="NewBrands">Les marques à enregistrer</param>
        /// <param name="NewFamilies">Les familles à enregistrer</param>
        /// <param name="NewSubFamilies">Les sous-familles à enregistrer</param>
        /// <param name="NewArticles">Les articles à enregistrer</param>
        /// <param name="NamesakeStrategyChoosed">La stratégie de dédoublonnage choisie</param>
        /// <returns></returns>
        private async Task SaveImportedData(Dictionary<string, Marques> NewBrands, 
            Dictionary<string, Familles> NewFamilies, Dictionary<string, SousFamilles> NewSubFamilies,
            Dictionary<Articles, Articles> NewArticles, NamesakeStrategy NamesakeStrategyChoosed)
        {
            StatusText.Text = "Import des données...";

            ImportProgress.Value = 0;
            ImportProgress.Maximum = NewBrands.Count;
            foreach (var Marques in NewBrands.Values)
            {
                ImportProgress.Value++;
                StatusText.Text = "Import des marques " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                await Task.Run(() => DaoMarque.Save(Marques));
            }

            ImportProgress.Value = 0;
            ImportProgress.Maximum = NewFamilies.Count;
            foreach (var Familles in NewFamilies.Values)
            {
                ImportProgress.Value++;
                StatusText.Text = "Import des familles " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                await Task.Run(() => DaoFamille.Save(Familles));
            }

            ImportProgress.Value = 0;
            ImportProgress.Maximum = NewSubFamilies.Count;
            foreach (var SousFamille in NewSubFamilies.Values)
            {
                ImportProgress.Value++;
                StatusText.Text = "Import des sous-familles " + ImportProgress.Value + "/" + ImportProgress.Maximum;
                await Task.Run(() => DaoSousFamille.Save(SousFamille));
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
                    await Task.Run(() => DaoArticle.Create(ArticlePair.Key));
                }
                else
                {
                    if (NamesakeStrategyChoosed == NamesakeStrategy.Replace)
                    {
                        await Task.Run(() => DaoArticle.Update(ArticlePair.Key));
                    }
                }
            }
        }

        /// <summary>
        /// Trouve les doublons entre les données à importer et la base
        /// </summary>
        /// <param name="ArticlesDtosRead">Les données à importer</param>
        /// <param name="NewFamilies">Les doublons des familles</param>
        /// <param name="NewBrands">Les doublons des marques</param>
        /// <param name="NewSubFamilies">Les doublons des sous-familles</param>
        /// <returns><c>true</c> si l'import est annulé, <c>false</c> sinon</returns>
        private bool FindDuplicates(List<ArticlesDto> ArticlesDtosRead, Dictionary<string, Familles> NewFamilies, 
            Dictionary<string, Marques> NewBrands, Dictionary<string, SousFamilles> NewSubFamilies)
        {
            var NameSakeErrorBuilder = new StringBuilder();
            int DuplicateFamilyCount = 0, DuplicateBrandCount = 0, DuplicateSubFamilyCount = 0;
            ImportProgress.Maximum = ArticlesDtosRead.Count * 2;
            StatusText.Text = "Dédoublonnage...";
            ImportProgress.Value = 0;
            foreach (var ArticlesDto in ArticlesDtosRead)
            {
                ImportProgress.Value += 1;

                var FamilyName = ArticlesDto.FamilyName;
                if (!NewFamilies.ContainsKey(FamilyName))
                {
                    var FamilyNameSake = DaoFamille.GetFamilleByName(FamilyName);
                    if (FamilyNameSake.Count > 0)
                    {
                        DuplicateFamilyCount += FamilyNameSake.Count;
                        NewFamilies.Add(FamilyName, FamilyNameSake[0]);
                    }
                }

                var BrandName = ArticlesDto.BrandName;
                if (!NewBrands.ContainsKey(BrandName))
                {
                    var BrandNameSake = DaoMarque.GetBrandByName(BrandName);
                    if (BrandNameSake.Count > 0)
                    {
                        DuplicateBrandCount += BrandNameSake.Count;
                        NewBrands.Add(BrandName, BrandNameSake[0]);
                    }
                }

                var SubFamilyName = ArticlesDto.FamilyName;
                if (!NewSubFamilies.ContainsKey(SubFamilyName))
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
                    DialogResult = DialogResult.Cancel;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Lit le fichier dont le chemin est passé en paramètres et sort les annomalies du fichiers, les doublons dans
        /// le fichier et le resultats. Les lignes avec des annomalies ne sont pas envoyés dans le resultat.
        /// </summary>
        /// <param name="ChoosedFilePath">Le chemin du fichier</param>
        /// <param name="ExistingArticlesCountByRef">Dictionnaire des doublons</param>
        /// <param name="Annomalies">Les annomalies du fichier</param>
        /// <param name="Result">Les lignes du fichiers</param>
        private static void ReadFile(string ChoosedFilePath, Dictionary<string, int> ExistingArticlesCountByRef, 
            Dictionary<int, string> Annomalies, List<ArticlesDto> Result)
        {
            CsvReader.ReadFile(ChoosedFilePath, (Strings, LineNumber) =>
            {
                try
                {
                    var RefArticle = Strings[1];
                    int PreviousLineDefinition;
                    if (ExistingArticlesCountByRef.TryGetValue(RefArticle, out PreviousLineDefinition))
                    {
                        Annomalies.Add(LineNumber,
                            String.Format("Réference d'article en doublon (précédement défini à la ligne {0})",
                                PreviousLineDefinition));
                    }
                    else
                    {
                        ExistingArticlesCountByRef.Add(RefArticle, LineNumber);
                        Result.Add(ArticlesDto.FromRawData(
                            // Description;Ref;Marque;Famille;Sous-Famille;Prix H.T.
                            RefArticle, Strings[0], Strings[3], Strings[4], Strings[2], Strings[5]));
                    }
                }
                catch (ParsingException ParsingException)
                {
                    Annomalies.Add(LineNumber, ParsingException.Message);
                }
            });
        }

        /// <summary>
        /// Met la fenêtre en mode 'chargement' pour notifier l'utilisateur qu'une opération est en cours,
        /// le curseur est changé et les boutons du formulaires sont grisés
        /// </summary>
        /// <param name="IsBusy">Si <c>true</c> le mode attente est activé, sinon il est retiré</param>
        private void SetBusy(bool IsBusy)
        {
            bool IsNotBusy = ! IsBusy;
            //Désactivation / Activation du bouton 'X' pour fermer la fenêtre
            ControlBox = IsNotBusy;
            AppendModeButton.Enabled = IsNotBusy;
            EreaseModeButton.Enabled = IsNotBusy;
            SelectCsvButton.Enabled = IsNotBusy;
            Application.UseWaitCursor = IsBusy;
        }
        
        //************************************************** EVENT **************************************************//
        
        /// <summary>
        /// Appelé au clique du bouton 'Importer en mode en mode ajout', lance l'importation en mode ajout
        /// (sans suppression de données).
        /// </summary>
        /// <param name="Sender">Non utilisé</param>
        /// <param name="Event">Non utilisé</param>
        private void AppendModeButton_Click(object Sender, EventArgs Event)
        {
            Import(false);
        }

        /// <summary>
        /// Appelé au clique du bouton 'Importer en mode écrasement', ce mode nécessite la remise à 0 de la base,
        /// un avertissement est donc affiché avant de lancer l'import.
        /// </summary>
        /// <param name="Sender">Non utilisé</param>
        /// <param name="Event">Non utilisé</param>
        private void EreaseModeButton_Click(object Sender, EventArgs Event)
        {
            var ConfirmResult =  MessageBox.Show("Cette action va écraser la base, êtes-vous sur de continuer ?",
                "Confirmation",
                MessageBoxButtons.YesNo);
            if (ConfirmResult == DialogResult.Yes)
            {
                Import(true);
            }
        }
        
        /// <summary>
        /// Appelé au clique du bouton 'Parcourir', ouvre une fenêtre pour sélectionner le fichier CSV à importer.
        /// </summary>
        /// <param name="Sender">Non utilisé</param>
        /// <param name="Event">Non utilisé</param>
        private void SelectCsvButton_Click(object Sender, EventArgs Event)
        {
            var OpenFileDialog = new OpenFileDialog
            {
                Title = "Choisir un fichier csv à importer",
                DefaultExt = "csv",
                Filter = "csv files (*.csv)|*.csv",
                CheckFileExists = true,
                CheckPathExists = true
            };
            var Result = OpenFileDialog.ShowDialog();
            if (Result == DialogResult.OK)
            {
                StatusText.Text = "Prêt à importer";
                EreaseModeButton.Enabled = true;
                AppendModeButton.Enabled = true;
                PathChoosedFile.Text = OpenFileDialog.FileName;
            }
        }
    }

    /// <summary>
    /// Enumération interne représentant la stratégie de dédoublonnage choisie
    /// </summary>
    internal enum NamesakeStrategy
    {
        //Les doublons existants sont remplacés
        Replace,
        //Les doublons ne sont pas importés
        Ignore
    }
}
