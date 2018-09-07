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
            LanguageState = ((Persistent.GetValue("language", "english") == "french") ? LanguageStates.French : LanguageStates.English);
        }

        public LanguageStates LanguageState { get; set; } = LanguageStates.English;

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
            LanguageState = (LanguageState == LanguageStates.English) ? LanguageStates.French : LanguageStates.English;

            Persistent.SetValue("language", LanguageState.ToString().ToLower());
            App.GetTranslation.SetLanguage(StateToLanguage[LanguageState]);

            NotifyPropertyChanged(nameof(LanguageState));
        }

        private Dictionary<LanguageStates, string> StateToLanguage = new Dictionary<LanguageStates, string>()
        {
            { LanguageStates.English, "English" },
            { LanguageStates.French, "Français" },
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
