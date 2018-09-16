using NetTools;
using Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppCSHtml5
{
    public class Language : ObjectBase, ILanguage
    {
        private enum ErrorCodes
        {
            AnyError = -1,
            Success = 0,
            Error,
        }

        public Language()
        {
            string SystemLanguage = Translation.Selected;
            Debug.WriteLine("System set the language to " + SystemLanguage);

            string UserLanguage = Persistent.GetValue("language", null);
            if (UserLanguage == null)
                UserLanguage = SystemLanguage;
            else
            {
                Debug.WriteLine("User set the language to " + UserLanguage);

                Translation TranslationObject = GetTranslation;
                if (TranslationObject != null)
                {
                    Debug.WriteLine("Setting language to " + UserLanguage);
                    TranslationObject.SetLanguage(UserLanguage);
                }
                else
                {
                    Translation.SetSelected(UserLanguage);
                    UserLanguage = Translation.Selected;
                }
            }

            Debug.WriteLine("Language: " + UserLanguage);
            LanguageState = ((UserLanguage == "fr-FR") ? LanguageStates.French : LanguageStates.English);
        }

        public LanguageStates LanguageState { get; set; }

        public bool IsTranslated
        {
            get
            {
                if (Window.Current.Content is Page CurrentPage)
                {
                    string Name = CurrentPage.GetType().Name;
                    if (UntranslatedPageNames[LanguageState].Contains(Name))
                        return false;
                }

                return true;
            }
        }

        private Dictionary<LanguageStates, List<string>> UntranslatedPageNames = new Dictionary<LanguageStates, List<string>>()
        {
            {
                LanguageStates.English, new List<string>()
                {
                }
            },
            {
                LanguageStates.French, new List<string>()
                {
                    nameof(eqmlp_doc_installationPage),
                    nameof(eqmlp_doc_privacyPage),
                    nameof(eqmlp_doc_registration_urlPage),
                    nameof(eqmlp_doc_server_administrationPage),
                    nameof(eqmlp_doc_skinsPage),
                    nameof(eqmlp_doc_tables_structurePage),
                    nameof(eqmlp_doc_uninstallationPage),
                    nameof(eqmlp_doc_updatePage),
                    nameof(eqmlp_doc_viewsPage),
                }
            },
        };

        public void On_Switch(PageNames pageName, string sourceName, string sourceContent)
        {
            LanguageStates NewState = (LanguageState == LanguageStates.English) ? LanguageStates.French : LanguageStates.English;
            string LanguageName = StateToLanguage[NewState];
            App.GetTranslation.SetLanguage(LanguageName);
            string UserLanguage = Translation.Selected;

            Persistent.SetValue("language", UserLanguage);
            LanguageState = ((UserLanguage == "fr-FR") ? LanguageStates.French : LanguageStates.English);

            NotifyPropertyChanged(nameof(LanguageState));
        }

        private void OnLanguageSwitched(LanguageStates newState)
        {
        }

        private Dictionary<LanguageStates, string> StateToLanguage = new Dictionary<LanguageStates, string>()
        {
            { LanguageStates.English, "en-US" },
            { LanguageStates.French, "fr-FR" },
        };

        public static string ReplaceHtml(string text)
        {
            string Result = text;
            Result = Result.Replace("&nbsp;", "\u00A0");

            return Result;
        }

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
