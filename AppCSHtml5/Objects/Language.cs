using Database;
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
    public class Language : ILanguage, INotifyPropertyChanged
    {
        public Language()
        {
            LanguageStrings[LanguageStates.English].Add("home caps", "HOME");
            LanguageStrings[LanguageStates.French].Add("home caps", "ACCUEIL");
            LanguageStrings[LanguageStates.English].Add("login caps", "LOGIN");
            LanguageStrings[LanguageStates.French].Add("login caps", "S'IDENTIFIER");
            LanguageStrings[LanguageStates.English].Add("logout caps", "LOGOUT");
            LanguageStrings[LanguageStates.French].Add("logout caps", "QUITTER");
            LanguageStrings[LanguageStates.English].Add("news caps", "NEWS");
            LanguageStrings[LanguageStates.French].Add("news caps", "NOUVEAUTÉS");
            LanguageStrings[LanguageStates.English].Add("products caps", "PRODUCTS");
            LanguageStrings[LanguageStates.French].Add("products caps", "PRODUITS");
            LanguageStrings[LanguageStates.English].Add("download caps", "DOWNLOAD");
            LanguageStrings[LanguageStates.French].Add("download caps", "TÉLÉCHARGER");
            LanguageStrings[LanguageStates.English].Add("support caps", "SUPPORT");
            LanguageStrings[LanguageStates.French].Add("support caps", "ASSISTANCE");
            LanguageStrings[LanguageStates.English].Add("contact caps", "CONTACT");
            LanguageStrings[LanguageStates.French].Add("contact caps", "CONTACT");
            LanguageStrings[LanguageStates.English].Add("home", "Home");
            LanguageStrings[LanguageStates.French].Add("home", "Accueil");
            LanguageStrings[LanguageStates.English].Add("login", "Login");
            LanguageStrings[LanguageStates.French].Add("login", "S'identifier");
            LanguageStrings[LanguageStates.English].Add("news", "News");
            LanguageStrings[LanguageStates.French].Add("news", "Nouveautés");
            LanguageStrings[LanguageStates.English].Add("products", "Products");
            LanguageStrings[LanguageStates.French].Add("products", "Produits");
            LanguageStrings[LanguageStates.English].Add("download", "Download");
            LanguageStrings[LanguageStates.French].Add("download", "Télécharger");
            LanguageStrings[LanguageStates.English].Add("support", "Support");
            LanguageStrings[LanguageStates.French].Add("support", "Assistance");
            LanguageStrings[LanguageStates.English].Add("choose", "Read in:");
            LanguageStrings[LanguageStates.French].Add("choose", "Lire en :");
            LanguageStrings[LanguageStates.English].Add("other-language", "French");
            LanguageStrings[LanguageStates.French].Add("other-language", "Anglais");
            LanguageStrings[LanguageStates.English].Add("home-title", "Welcome to Numbatsoft.");
            LanguageStrings[LanguageStates.French].Add("home-title", "Bienvenue sur Numbatsoft.");
            LanguageStrings[LanguageStates.English].Add("home-text-1", "Here you will find several tools and applications.\u00A0 See this link for a list, each described in detail.\u00A0 You can also directly go to the download page if you already know what you are looking for.");
            LanguageStrings[LanguageStates.French].Add("home-text-1", "Vous pourrez trouver ici plusieurs outils et applications. Consultez ce lien pour une liste où chacun est décrit en détail. Vous pouvez également vous rendre directement sur la page de téléchargement si vous savez déjà ce que vous recherchez.");
            LanguageStrings[LanguageStates.English].Add("home-text-2", "Every product found at Numbatsoft can be downloaded and tested for free.\u00A0 The free version is full - featured and does not require registration to be used.\u00A0 It will not show popup windows or any other annoying reminder about this site.\u00A0 However, registering gives you additional benefits like flexibility and support, and entitles you to obtain the latest patches and bug fixes.");
            LanguageStrings[LanguageStates.French].Add("home-text-2", "Chaque produit disponible sur Numbatsoft peut être téléchargé et testé gratuitement.La version gratuite inclut toutes les fonctionnalités et ne requiert pas d'être enregistré pour pouvoir être utilisée. Elle n'affiche pas de fenêtre intempestive et autres rappels gênants à propos de ce site.Cependant, s'enregistrer vous apporte des avantages supplémentaires tels qu'une certaine flexibilité et une assistance technique, et vous permet d'obtenir les derniers correctifs.");
            LanguageStrings[LanguageStates.English].Add("home-text-3", "Please check the release notes of each patch to see if your problem can be corrected, or if the feature you're looking for has been added.");
            LanguageStrings[LanguageStates.French].Add("home-text-3", "Veuillez consulter les notes d'utilisation à chaque nouvelle version pour vérifier si votre problème est corrigé, ou si la fonctionnalité que vous désirez a été ajoutée.");
            LanguageStrings[LanguageStates.English].Add("home-text-4", "I hope you will enjoy what you find here.\u00A0 In any case, you are encouraged to send feedback.\u00A0 All contributions are welcome!");
            LanguageStrings[LanguageStates.French].Add("home-text-4", "J'espère que vous apprécierez tout ce que vous trouvez ici. En tous cas, vous êtes vivement invité à envoyer votre opinion. Toutes les contributions sont les bienvenues !");
            LanguageStrings[LanguageStates.English].Add("last-news-title", "Latest news");
            LanguageStrings[LanguageStates.French].Add("last-news-title", "Dernières nouvelles");
            LanguageStrings[LanguageStates.English].Add("archive-news-title", "News archives");
            LanguageStrings[LanguageStates.French].Add("archive-news-title", "Précédement");
            LanguageStrings[LanguageStates.English].Add("read-more", "Read more");
            LanguageStrings[LanguageStates.French].Add("read-more", "En savoir plus");
            LanguageStrings[LanguageStates.English].Add("login-title", "Login information");
            LanguageStrings[LanguageStates.French].Add("login-title", "Informations d'identification");
            LanguageStrings[LanguageStates.English].Add("login-text", "Please fill the form below to log in.\u00A0 If you don't have a login name and password, please register.\u00A0 If you cannot remember your name or password, you can recover them.");
            LanguageStrings[LanguageStates.French].Add("login-text", "Veuillez entrer les informations requises ci-dessous pour vous identifier. Si vous n'avez pas un identifiant avec mot de passe, veuillez vous enregistrer. Si vous ne pouvez vous rappeler de votre identifiant, ou de votre mot de passe, vous pouvez les récupérer.");
            LanguageStrings[LanguageStates.English].Add("name-colon", "Name:");
            LanguageStrings[LanguageStates.French].Add("name-colon", "Nom :");
            LanguageStrings[LanguageStates.English].Add("password-colon", "Password:");
            LanguageStrings[LanguageStates.French].Add("password-colon", "Mot de passe :");
            LanguageStrings[LanguageStates.English].Add("remember-me", "Remember me on this computer");
            LanguageStrings[LanguageStates.French].Add("remember-me", "Se rappeller de moi sur cet ordinateur");
            LanguageStrings[LanguageStates.English].Add("login-failed-title", "Login failed");
            LanguageStrings[LanguageStates.French].Add("login-failed-title", "L'identification a échoué");
            LanguageStrings[LanguageStates.English].Add("login-failed-text-1", "Sorry, the name or password you entered is not correct.");
            LanguageStrings[LanguageStates.French].Add("login-failed-text-1", "Désolé, le nom ou le mot de passe que vous avez entré sont incorrects.");
            LanguageStrings[LanguageStates.English].Add("login-failed-text-2", "Click here if you want to try again.\u00A0 If you think you might have lost your login information, you can try to recover them.");
            LanguageStrings[LanguageStates.French].Add("login-failed-text-2", "Cliquez ici si vous voulez essayer à nouveau. Si vous pensez que vous avez perdu vos informations d'identification, vous pouvez essayer de les récupérer.");
            LanguageStrings[LanguageStates.English].Add("logged-as", "Logged as:");
            LanguageStrings[LanguageStates.French].Add("logged-as", "Identifiant :");
            LanguageStrings[LanguageStates.English].Add("account-title", "Your account");
            LanguageStrings[LanguageStates.French].Add("account-title", "Votre compte");
            LanguageStrings[LanguageStates.English].Add("email-colon", "Email:");
            LanguageStrings[LanguageStates.French].Add("email-colon", "Courriel :");
            LanguageStrings[LanguageStates.English].Add("recovery-colon-1", "Recovery question:");
            LanguageStrings[LanguageStates.French].Add("recovery-colon-1", "Question :");
            LanguageStrings[LanguageStates.English].Add("recovery-colon-2", "");
            LanguageStrings[LanguageStates.French].Add("recovery-colon-2", "(Pour récupération)");
            LanguageStrings[LanguageStates.English].Add("account-management-colon", "Account management:");
            LanguageStrings[LanguageStates.French].Add("account-management-colon", "Gestion du compte :");
            LanguageStrings[LanguageStates.English].Add("change-password", "change password");
            LanguageStrings[LanguageStates.French].Add("change-password", "changer le mot de passe");
            LanguageStrings[LanguageStates.English].Add("change-email", "change email address");
            LanguageStrings[LanguageStates.French].Add("change-email", "changer l'addresse courriel");
            LanguageStrings[LanguageStates.English].Add("change-recovery", "change recovery question and answer");
            LanguageStrings[LanguageStates.French].Add("change-recovery", "changer la question de récupération et sa réponse");
            LanguageStrings[LanguageStates.English].Add("change-password-title", "Change password");
            LanguageStrings[LanguageStates.French].Add("change-password-title", "Changer le mot de passe");
            LanguageStrings[LanguageStates.English].Add("change-password-text", "Please enter your current password, followed by the new password with confirmation.");
            LanguageStrings[LanguageStates.French].Add("change-password-text", "Veuillez entrer votre mot de passe actuel, suivi par le nouveau mot de passe avec confirmation.");
            LanguageStrings[LanguageStates.English].Add("current-password-colon", "Current password:");
            LanguageStrings[LanguageStates.French].Add("current-password-colon", "Mot de passe actuel :");
            LanguageStrings[LanguageStates.English].Add("new-password-colon", "New password:");
            LanguageStrings[LanguageStates.French].Add("new-password-colon", "Nouveau :");
            LanguageStrings[LanguageStates.English].Add("confirm-password-colon", "Confirm password:");
            LanguageStrings[LanguageStates.French].Add("confirm-password-colon", "Confirmation :");
            LanguageStrings[LanguageStates.English].Add("change", "Change now");
            LanguageStrings[LanguageStates.French].Add("change", "Changer");
            LanguageStrings[LanguageStates.English].Add("change-email-title", "change email address");
            LanguageStrings[LanguageStates.French].Add("change-email-title", "changer l'addresse courriel");
            LanguageStrings[LanguageStates.English].Add("change-recovery-title", "change recovery question");
            LanguageStrings[LanguageStates.French].Add("change-recovery-title", "changer la question de récupération");
            LanguageStrings[LanguageStates.English].Add("change-password-failed-title", "Changing the password failed");
            LanguageStrings[LanguageStates.French].Add("change-password-failed-title", "Le changement de mot de passe a échoué");
            LanguageStrings[LanguageStates.English].Add("change-password-failed-text-1", "Sorry, you must enter your current password to be able to change it.");
            LanguageStrings[LanguageStates.French].Add("change-password-failed-text-1", "Désolé, vous devez entrer votre mot de passe actuel pour pouvoir le changer.");
            LanguageStrings[LanguageStates.English].Add("change-password-failed-text-2", "Sorry, you did not enter a new password or did not confirm it.");
            LanguageStrings[LanguageStates.French].Add("change-password-failed-text-2", "Désolé, vous n'avez pas entré de nouveau mot de passe, ou bien sa confirmation.");
            LanguageStrings[LanguageStates.English].Add("change-password-failed-text-3", "Sorry, you misspelled your password in the confirmation box.");
            LanguageStrings[LanguageStates.French].Add("change-password-failed-text-3", "Désolé, vous avez entré un mot de passe incorrect dans la ligne de confirmation.");
            LanguageStrings[LanguageStates.English].Add("change-password-failed-text-4", "Sorry, the site encountered an unrecoverable error.");
            LanguageStrings[LanguageStates.French].Add("change-password-failed-text-4", "Désolé, une erreur irrécupérable c'est produite sur le site.");
            LanguageStrings[LanguageStates.English].Add("change-password-failed-text-5", "Sorry, you misspelled your current password.");
            LanguageStrings[LanguageStates.French].Add("change-password-failed-text-5", "Désolé, vous avez entré un mot de passe actuel erroné.");
            LanguageStrings[LanguageStates.English].Add("retry", "Click here to try again.");
            LanguageStrings[LanguageStates.French].Add("retry", "Cliquez ici pour essayer à nouveau.");
            LanguageStrings[LanguageStates.English].Add("change-password-success-title", "Password changed");
            LanguageStrings[LanguageStates.French].Add("change-password-success-title", "Le changement de mot de passe a réussi");
            LanguageStrings[LanguageStates.English].Add("change-password-success-text", "Congratulation, your password has been changed.");
            LanguageStrings[LanguageStates.French].Add("change-password-success-text", "Félicitation, votre mot de passe a bien été changé.");
            LanguageStrings[LanguageStates.English].Add("return-home", "Click here to return to your account summary.");
            LanguageStrings[LanguageStates.French].Add("return-home", "Cliquez ici pour retourner à la page Sommaire de votre compte.");

            LanguagePageStrings[LanguageStates.English].Add("home", "Welcome!");
            LanguagePageStrings[LanguageStates.French].Add("home", "Bienvenue !");
            LanguagePageStrings[LanguageStates.English].Add("news", "News");
            LanguagePageStrings[LanguageStates.French].Add("news", "Nouveautés");
            LanguagePageStrings[LanguageStates.English].Add("login", "Login");
            LanguagePageStrings[LanguageStates.French].Add("login", "Identification");
            LanguagePageStrings[LanguageStates.English].Add("login-failed", "Login");
            LanguagePageStrings[LanguageStates.French].Add("login-failed", "Identification");
            LanguagePageStrings[LanguageStates.English].Add("account", "Account summary");
            LanguagePageStrings[LanguageStates.French].Add("account", "Sommaire du compte");
            LanguagePageStrings[LanguageStates.English].Add("change-password", "Account management");
            LanguagePageStrings[LanguageStates.French].Add("change-password", "Gestion du compte");
            LanguagePageStrings[LanguageStates.English].Add("change-email", "Account management");
            LanguagePageStrings[LanguageStates.French].Add("change-email", "Gestion du compte");
            LanguagePageStrings[LanguageStates.English].Add("change-recovery", "Account management");
            LanguagePageStrings[LanguageStates.French].Add("change-recovery", "Gestion du compte");
            LanguagePageStrings[LanguageStates.English].Add("change-password-failed-#1", "Account management");
            LanguagePageStrings[LanguageStates.French].Add("change-password-failed-#1", "Gestion du compte");
            LanguagePageStrings[LanguageStates.English].Add("change-password-failed-#2", "Account management");
            LanguagePageStrings[LanguageStates.French].Add("change-password-failed-#2", "Gestion du compte");
            LanguagePageStrings[LanguageStates.English].Add("change-password-failed-#3", "Account management");
            LanguagePageStrings[LanguageStates.French].Add("change-password-failed-#3", "Gestion du compte");
            LanguagePageStrings[LanguageStates.English].Add("change-password-failed-#5", "Account management");
            LanguagePageStrings[LanguageStates.French].Add("change-password-failed-#5", "Gestion du compte");
            LanguagePageStrings[LanguageStates.English].Add("change-password-success", "Account management");
            LanguagePageStrings[LanguageStates.French].Add("change-password-success", "Gestion du compte");

            State = ((Persistent.GetValue("language", "english") == "french") ? LanguageStates.French : LanguageStates.English);
        }

        public LanguageStates State { get; set; } = LanguageStates.English;
        public Dictionary<string, string> Strings { get { return LanguageStrings[State]; } }
        public Dictionary<string, string> PageStrings { get { return LanguagePageStrings[State]; } }
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

        public IList<INewsEntry> AllNews
        {
            get
            {
                GetNews();
                return _AllNews;
            }
        }
        private IList<INewsEntry> _AllNews = new ObservableCollection<INewsEntry>();

        private bool IsAllNewsParsed;

        private Dictionary<LanguageStates, Dictionary<string, string>> LanguageStrings { get; } = new Dictionary<LanguageStates, Dictionary<string, string>>()
        {
            { LanguageStates.English, new Dictionary<string, string>() },
            { LanguageStates.French, new Dictionary<string, string>() },
        };

        private Dictionary<LanguageStates, Dictionary<string, string>> LanguagePageStrings { get; } = new Dictionary<LanguageStates, Dictionary<string, string>>()
        {
            { LanguageStates.English, new Dictionary<string, string>() },
            { LanguageStates.French, new Dictionary<string, string>() },
        };

        public void On_Switch(string pageName, string sourceName, string sourceContent)
        {
            State = (State == LanguageStates.English) ? LanguageStates.French : LanguageStates.English;
            foreach (NewsEntry Item in _AllNews)
                Item.SelectLanguage(State);

            Persistent.SetValue("language", State.ToString().ToLower());

            NotifyPropertyChanged(nameof(State));
            NotifyPropertyChanged(nameof(Strings));
            NotifyPropertyChanged(nameof(PageStrings));
        }

        private void GetNews()
        {
            if (IsAllNewsParsed)
                return;

            IsAllNewsParsed = true;
            GetNews(OnNewsReceived);
        }

        private void OnNewsReceived(bool success, object result)
        {
            if (!success)
                return;

            List<Dictionary<string, string>> NewsList = (List<Dictionary<string, string>>)result;
            Debug.WriteLine($"{NewsList.Count} news entries received");

            foreach (Dictionary<string, string> Item in NewsList)
            {
                NewsEntry NewEntry = new NewsEntry(State, Item["enu_summary"], Item["enu_content"], Item["fra_summary"], Item["fra_content"]);
                _AllNews.Add(NewEntry);
            }

            NotifyPropertyChanged(nameof(LastNews));
            NotifyPropertyChanged(nameof(ArchiveNews));
        }

        #region Operations
        private void GetNews(Action<bool, object> callback)
        {
            Database.Completed += OnGetNewsCompleted;
            Database.Query(new DatabaseQueryOperation("get news entries", "query_2.php", new Dictionary<string, string>(), callback));
        }

        private void OnGetNewsCompleted(object sender, CompletionEventArgs e)
        {
            Debug.WriteLine("OnGetNewsCompleted notified");
            Database.Completed -= OnGetNewsCompleted;

            Action<bool, object> Callback = e.Operation.Callback;

            List<Dictionary<string, string>> Result;
            if ((Result = Database.ProcessMultipleResponse(e.Operation, new List<string>() { "enu_summary", "enu_content", "fra_summary", "fra_content" })) != null)
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(true, Result));
            else
                Windows.UI.Xaml.Window.Current.Dispatcher.BeginInvoke(() => Callback(false, null));
        }
        #endregion

        public static string ReplaceHtml(string text)
        {
            string Result = text;
            Result = Result.Replace("&nbsp;", "\u00A0");

            return Result;
        }

        private Database.Database Database = new Database.Database();

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
