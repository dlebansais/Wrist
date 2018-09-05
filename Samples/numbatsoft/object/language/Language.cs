using NetTools;
using Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Language : ILanguage
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
            Error,
        }

        public Language()
        {
            LanguageState = ((Persistent.GetValue("language", "english") == "french") ? LanguageStates.French : LanguageStates.English);
            //Database.DebugWriteResponse = true;

            InitSimulation();
        }

        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ILogin GetLogin { get { return App.GetLogin; } }
        public IEqmlp GetEqmlp { get { return App.GetEqmlp; } }

        public LanguageStates LanguageState { get; set; } = LanguageStates.English;

        public INewsEntry LastNews
        {
            get
            {
                GetNews();

                if (_AllNews.Count > 0)
                    return _AllNews[0];

                return null;
            }
        }

        public INewsEntry ArchiveNews
        {
            get
            {
                GetNews();

                if (_AllNews.Count > 1)
                    return _AllNews[1];

                return null;
            }
        }

        public ObservableCollection<INewsEntry> AllNews
        {
            get
            {
                GetNews();
                return _AllNews;
            }
        }
        private ObservableCollection<INewsEntry> _AllNews = new ObservableCollection<INewsEntry>();

        private bool IsAllNewsParsed;

        public void On_Switch(PageNames pageName, string sourceName, string sourceContent)
        {
            LanguageState = (LanguageState == LanguageStates.English) ? LanguageStates.French : LanguageStates.English;
            foreach (NewsEntry Item in _AllNews)
                Item.SelectLanguage(LanguageState);

            Persistent.SetValue("language", LanguageState.ToString().ToLower());
            App.GetTranslation.SetLanguage(StateToLanguage[LanguageState]);

            NotifyPropertyChanged(nameof(LanguageState));
        }

        private Dictionary<LanguageStates, string> StateToLanguage = new Dictionary<LanguageStates, string>()
        {
            { LanguageStates.English, "English" },
            { LanguageStates.French, "Français" },
        };

        private void GetNews()
        {
            if (IsAllNewsParsed)
                return;

            IsAllNewsParsed = true;
            GetNews(OnNewsReceived);
        }

        private void OnNewsReceived(int error, object result)
        {
            if (error != (int)ErrorCodes.Success)
                return;

            List<Dictionary<string, string>> NewsList = (List<Dictionary<string, string>>)result;
            Debug.WriteLine($"{NewsList.Count} news entries received");

            foreach (Dictionary<string, string> Item in NewsList)
            {
                NewsEntry NewEntry = new NewsEntry(LanguageState, Item["created"], Item["enu_summary"], Item["enu_content"], Item["fra_summary"], Item["fra_content"]);
                _AllNews.Add(NewEntry);
            }

            NotifyPropertyChanged(nameof(LastNews));
            NotifyPropertyChanged(nameof(ArchiveNews));
        }

        public static string ReplaceHtml(string text)
        {
            string Result = text;
            Result = Result.Replace("&nbsp;", "\u00A0");

            return Result;
        }

        #region Operations
        private void GetNews(Action<int, object> callback)
        {
            Database.Completed += OnGetNewsCompleted;
            Database.Query(new DatabaseQueryOperation("get news entries", "query_2.php", new Dictionary<string, string>(), callback));
        }

        private void OnGetNewsCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnGetNewsCompleted notified");
            Database.Completed -= OnGetNewsCompleted;

            Action<int, object> Callback = e.Operation.Callback;

            List<Dictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "created", "enu_summary", "enu_content", "fra_summary", "fra_content" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.Success, Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback((int)ErrorCodes.AnyError, null));
        }
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            OperationHandler.Add(new OperationHandler("/request/query_2.php", OnQueryNews));
        }

        private List<Dictionary<string, string>> OnQueryNews(Dictionary<string, string> parameters)
        {
            List<Dictionary<string, string>> Result = new List<Dictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2015-07-14 10:37:53" },
                { "enu_summary", "Custom Controls updated" },
                { "enu_content", "A set of custom controls, on which I've been working for months, is finally available. It includes the three previous controls (Editable TextBlock, Dialog Validation and Extended TreeView) as well as new controls.\u00A0 " +
"These can be grouped in two categories: utility controls such as Tightfitting Tab Control, and specialized controls for solutions management.\u00A0 " +
"The later are used by Easly Solution Manager, available at www.easly.org." },
                { "fra_summary", "Contrôles personnalisés mis à jour" },
                { "fra_content", "Une suite de contrôles personnalisés, sur laquelle je travaille depuis plusieurs mois, est enfin disponible. Elle comprend les trois contrôles déjà disponibles (Editable TextBlock, Dialog Validation et Extended TreeView) plus toutes une séries de nouveaux contrôles. " +
"On peut les regrouper en deux catégories : une série de contrôles utilitaires, par exemple Tightfitting Tab Control, et des contrôles spécialisés dans la gestion de solutions. " +
"Ceux-ci sont utilisés par Easly Solution Manager, disponible sur www.easly.org." },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2015-03-28 11:15:17" },
                { "enu_summary", "Dialog Validation control added" },
                { "enu_content", "I just added a new custom control to the library. It displays buttons you see at the bottom of a dialog box (like OK or Cancel). It sounds simple and easy at first, but this control can display strings already localized from Windows, which is a great deal for me (even if my most recent programs are English only).\u00A0 " +
"It also simplifies the design of dialog boxes.\u00A0 " +
"Documentation will be available soon." },
                { "fra_summary", "Ajout du contrôle personnalisé Dialog Validation" },
                { "fra_content", "Je viens d'ajouter à la librairie un nouveau contrôle personnalisé. Il affiche les boutons que l'on peut trouver au bas des boîtes de dialogue (comme OK ou Annuler). Cela parait simple, mais ce contrôle peut afficher les chaînes de caractères déjà personnalisées de Windows, ce qui est très important pour moi (même si mes derniers programmes sont tous en anglais seul)." +
"Il simplifie également la création de boîtes de dialogue." +
"La documentation sera disponible prochainement." },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2015-03-26 16:11:12" },
                { "enu_summary", "New custom control" },
                { "enu_content", "A new and more ambitious custom control has been added to the library. It's a TreeView with additional features.\u00A0 " +
"Since the new control is a lot more complex than a mere editable TextBlock, the list of features will probably be available as a separate document, hopefully soon." },
                { "fra_summary", "Nouveau contrôle personnalisé" },
                { "fra_content", "Un nouveau contrôle personnalisé, plus ambitieux, a été ajouté à la librairie. Il s'agit d'un TreeView avec des fonctionnalités supplémentaires. " +
"Comme le nouveau contrôle est beaucoup plus complexe qu'un simple TextBlock éditable, la liste des fonctionnalités sera probablement disponible dans un document séparé, et j'espère très bientôt." },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2015-03-12 14:10:46" },
                { "enu_summary", "First custom control added" },
                { "enu_content", "Today I have added to the download section what I hope to be the first of a long list of WPF custom controls. It's the simplest of them, and I have two more in the queue.\u00A0 " +
"I intend to provide both a 32-bits and 64-bits version of each control, as well as the source code.\u00A0 " +
"Eventually I will also update the documentation section with a summary of best practices when writing WPF custom controls, since I've needed one and couldn't find it." },
                { "fra_summary", "Premier ajour d'un contrôle personnalisé" },
                { "fra_content", "Aujourd'hui j'ai ajouté à la section Téléchargement ce que j'espère être le premier d'une longue liste de contrôles WPF personnalisés. C'est le plus simple d'entre eux, et j'en ai deux autres en attente. " +
"J'ai l'intention de fournir à la fois une version 32-bits et une version 64-bits de chaque contrôle, ainsi que leur code source. " +
"Ultérieurement je mettrais également à jour la section documentation avec un résumé des meilleures pratiques à suivre quand on écrit un contrôle WPF personnalisé, car j'en ai eu besoin et je n'en ai pas trouvé." },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2011-03-22 18:14:16" },
                { "enu_summary", "Everquest Multilog Parser is now live!" },
                { "enu_content", "This application brings a unique feature to Everquest players: the ability to parse raids accurately, in real-time.&nbsp; Everquest Multilog Parser can be <a href=\"#root#/download/eqmlp/eqmlp.php\">downloaded</a> for free, and additional services are available.&nbsp; See the <a href=\"#root#/products/eqmlp/eqmlp.php\">product description</a> for details." },
                { "fra_summary", "Everquest Multilog Parser est disponible !" },
                { "fra_content", "Cette application apporte une fonctionnalité unique aux joueurs d'Everquest : la possibilité de parser les raids avec précision, en temps réel. Everquest Multilog Parser peut être <a href=\"#root#/download/eqmlp/eqmlp.php\">téléchargé</a> gratuitement, et des services supplémentaires sont disponibles. Consultez la <a href=\"#root#/products/eqmlp/eqmlp.php\">description du produit</a> pour en connaitre le détail." },
            });

            return Result;
        }
        #endregion

        private Database Database = Database.Current;

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        ///     Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        internal void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
