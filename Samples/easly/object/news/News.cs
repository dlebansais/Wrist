using NetTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppCSHtml5
{
    public class News : ObjectBase, INews
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
            Error,
        }

        public News()
        {
            InitSimulation();
        }

        public INewsEntry NewsArchive0 { get { return GetNewsArchive(0); } }
        public INewsEntry NewsArchive1 { get { return GetNewsArchive(1); } }
        public INewsEntry NewsArchive2 { get { return GetNewsArchive(2); } }
        public INewsEntry NewsArchive3 { get { return GetNewsArchive(3); } }

        public ObservableCollection<INewsEntry> AllNews
        {
            get
            {
                GetAllNews();
                return _AllNews;
            }
        }
        private ObservableCollection<INewsEntry> _AllNews = new ObservableCollection<INewsEntry>();

        private bool IsAllNewsParsed;

        private INewsEntry GetNewsArchive(int index)
        {
            GetAllNews();

            if (_AllNews.Count > index)
                return _AllNews[index];

            return null;
        }

        private void GetAllNews()
        {
            if (IsAllNewsParsed)
                return;

            IsAllNewsParsed = true;
            GetAllNews(OnNewsReceived);
        }

        private void OnNewsReceived(int error, object result)
        {
            if (error == (int)ErrorCodes.Success && result != null)
            {
                List<IDictionary<string, string>> NewsList = (List<IDictionary<string, string>>)result;

                foreach (IDictionary<string, string> Item in NewsList)
                {
                    NewsEntry NewEntry = new NewsEntry(Item["created"], Item["enu_summary"], Item["enu_content"], Item["fra_summary"], Item["fra_content"]);
                    _AllNews.Add(NewEntry);
                }

                NotifyPropertyChanged(nameof(NewsArchive0));
                NotifyPropertyChanged(nameof(NewsArchive1));
                NotifyPropertyChanged(nameof(NewsArchive2));
                NotifyPropertyChanged(nameof(NewsArchive3));
            }
        }

        #region Operations
        private void GetAllNews(Action<int, object> callback)
        {
            Database.Query(new DatabaseQueryOperation("get all news entries", "news/query_all_news.php", new Dictionary<string, string>(), (object sender, CompletionEventArgs e) => OnGetAllNewsCompleted(sender, e, callback)));
        }

        private void OnGetAllNewsCompleted(object sender, CompletionEventArgs e, Action<int, object> callback)
        {
            List<IDictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "created", "enu_summary", "enu_content", "fra_summary", "fra_content" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => callback((int)ErrorCodes.Success, Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => callback((int)ErrorCodes.AnyError, null));
        }
        #endregion

        #region Simulation
        private void InitSimulation()
        {
            if (NetTools.UrlTools.IsUsingRestrictedFeatures)
                return;

            OperationHandler.Add(new OperationHandler($"/{Database.QueryScriptPath}news/query_all_news.php", OnQueryNews));
        }

        private List<IDictionary<string, string>> OnQueryNews(IDictionary<string, string> parameters)
        {
            List<IDictionary<string, string>> Result = new List<IDictionary<string, string>>();

            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2015-07-14 15:13:36" },
                { "enu_summary", "Version 1.3.0.6238 available" },
                { "enu_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "A long awaited update! This version doesn't bring may new features, but a lot has happened behind the scene, with most of the code ported to generic or specialized custom controls.\u00A0 " +
                                 "In particular, the entire management of solutions is now just a huge custom control that uses its own separate code base.\u00A0 " +
                                 "Should now be able to focus more on a few more features I want to help with development, and then language features.")) },
                { "fra_summary", "Version 1.3.0.6238 disponible" },
                { "fra_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "Une mise à jour très attendue ! Cette version n'amène pas de nouvelles fonctionnalités, mais il y a eu de nombreux changements en interne, avec une bonne partie du code portée vers des contrôles personnalisés, certains génériques et d'autres spécialisés. " + 
                                 "En particulier, la gestion des solutions est à présent une énorme contrôle personnalisé qui utilise sa propre base de code séparée. " + 
                                 "Je devrais pouvoir à présent me concentrer sur quelques fonctionnalités dont j'ai besoin pour aider au développement, et ensuite des fonctionnalités du langage.")) },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2015-04-16 11:28:09" },
                { "enu_summary", "Some bug fixes" },
                { "enu_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "I have updated the current version of the Solution Manager to 1.3.0.1395. It contains some bug fixes and a few improvements in the compiler.\u00A0 " +
                                 "I've noticed that complex inheritance trees are not translated properly to C#, so I'm going to review that part. Also, the user interface could use some polishing and is now sufficiently close to the final result to allow me to specify it, in a backward way.\u00A0 " + 
                                 "And finally, I still have a few issues with drag & drop in the TreeView control so I will probably review that one too.")) },
                { "fra_summary", "Quelques bugs fixés" },
                { "fra_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "J'ai mis à jour la version courante du Solution Manager en 1.3.0.1395. Elle contient quelques fix et quelques améliorations dans le compilateur. " +
                                 "J'ai noté que les arbres d'héritage complexes ne sont pas transformés correctement en C#, et vais donc revoir cette partie. " +
                                 "De même, l'interface utilisateur pourrait être améliorée, et est suffisamment proche du résultat final pour me permettre de la spécifier, plus ou moins à posteriori. " +
                                 "Enfin, j'ai encore quelques problèmes avec le drag & drop du contrôle TreeView, donc je vais probablement revoir celui-ci également.")) },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2015-04-06 17:37:11" },
                { "enu_summary", "Version 1.3.0.766 released" },
                { "enu_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "This new version includes several bug fixes, and a new feature: the verification step at the end of the compilation can be skipped.\u00A0 " +
                                 "This is because I want to create more code and examples that compile, and right now it would take a lot of work for the verification step to succeed.")) },
                { "fra_summary", "Version 1.3.0.766 disponible" },
                { "fra_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "Cette nouvelle version contient plusieurs corrections de bugs, et une nouvelle fonctionnalité : la phase de vérification à la fin de la compilation peut être évitée. " +
                                 "Cela est nécessaire car je veux créer plus de code et d'exemples qui compilent, et à l'heure actuelle il y aurait beaucoup de travail pour que la phase de vérification puisse réussir.")) },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2015-03-27 14:19:11" },
                { "enu_summary", "Solution Manager update" },
                { "enu_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "A new version of the solution manager is available (1.2.0.25180). Check the bug list for details.\u00A0 " +
                                 "I have ported the editable text and tree view controls to fully separate custom controls (you can find them also on www.numbatsoft.com) and ported the docking manage to AvalonDock 2.6.\u00A0 " +
                                 "Unfortunately, I had to drop the expression theme in the process.")) },
                { "fra_summary", "Mise à jour de Solution Manager" },
                { "fra_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "Une nouvelle version de Solution Manager est disponible (1.2.0.25180). La buglist contient les détails. " +
                                 "J'ai porté les contrôles texte éditable et TreeView en contrôle personnalisés séparés, et le docking manager vers la version 2.6 de AvalonDock. " +
                                 "Malheureusement, il a fallu pour cela abandonner le thème expression.")) },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "created", "2015-03-04 14:44:53" },
                { "enu_summary", "Version 1.1.0.25301 released" },
                { "enu_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "<p>This version released today brings several improvements:</p>" +
                                 "<li>Display of comments, zoom.</li>" +
                                 "<li>Themes are implemented, although porting to AvalanDock 2.6 should improve them in the future.</li>" +
                                 "<li>Advanced class & library search(name, date, etc.)</li>" +
                                 "<li>Clicking on a compilation error puts the focus on the corresponding code(still a few bugs there)</li>" +
                                 "<li>Text replacement works and enables proper edit of the code.Intellisense will not be implemented any time soon, though.</li>" +
                                 "<p/>")) },
                { "fra_summary", "Version 1.1.0.25301 disponible" },
                { "fra_content", Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                 "<p>Cette version disponible aujourd'hui apporte un certain nombre d'améliorations :</p>" +
                                 "<li>Affichage des commentaires, zoom.</li>" +
                                 "<li>Les thèmes sont implémentés, bien qu'un portage vers AvalanDock 2.6 devrait encore améliorer les choses dans le futur.</li>" +
                                 "<li>Recherche avancée des classes & librairies (par nom, date, etc.)</li>" +
                                 "<li>Cliquer sur une erreur de compilation met le focus sur le code correspondant (bien qu'il y ait encore quelques bugs sur ce point)</li>" +
                                 "<li>Le remplacement de texte fonctionne et permet d'éditer correctement le code. Cependant, Intellisense ne sera pas implémenté de si tôt.</li>" +
                                 "<p/>" +
                                 "<p>La prochaine étape consistera à faciliter l'utilisation générale, et, je l'espère, une meilleure intégration avec le compilateur C#.</p>")) },
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
