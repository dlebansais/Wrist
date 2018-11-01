using Presentation;
using System.Collections.Generic;
using System.ComponentModel;
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
        }

        public LanguageStates LanguageState
        {
            get
            {
                Initialization();
                return _LanguageState;
            }
        }

        private LanguageStates _LanguageState  = (LanguageStates)(-1);

        private void Initialization()
        {
            if (_LanguageState != (LanguageStates)(-1))
                return;

            string SystemLanguage = Translation.Selected;

            string UserLanguage = Persistent.GetValue("language", null);
            if (UserLanguage == null)
                UserLanguage = SystemLanguage;
            else
            {
                Translation TranslationObject = GetTranslation;
                if (TranslationObject != null)
                    TranslationObject.SetLanguage(UserLanguage);
                else
                {
                    Translation.SetSelected(UserLanguage);
                    UserLanguage = Translation.Selected;
                }
            }

            _LanguageState = ((UserLanguage == "fr-FR") ? LanguageStates.French : LanguageStates.English);
        }

        public bool IsTranslated
        {
            get
            {
                Initialization();

                if (Window.Current.Content is Page CurrentPage)
                {
                    string Tag = CurrentPage.Tag as string;
                    if (!string.IsNullOrEmpty(Tag))
                    {
                        for (int i = (int)LanguageStates.English; i <= (int)LanguageStates.French; i++)
                        {
                            string TranslationSpecification = $"{(LanguageStates)i} only";
                            if (string.Compare(Tag, TranslationSpecification, true) == 0)
                                if (LanguageState != (LanguageStates)i)
                                    return false;
                                else
                                    break;
                        }
                    }
                }

                return true;
            }
        }

        public void On_Switch(PageNames pageName, string sourceName, string sourceContent)
        {
            Initialization();

            LanguageStates NewState = (LanguageState == LanguageStates.English) ? LanguageStates.French : LanguageStates.English;
            string LanguageName = StateToLanguage[NewState];
            App.GetTranslation.SetLanguage(LanguageName);
            string UserLanguage = Translation.Selected;

            Persistent.SetValue("language", UserLanguage);
            _LanguageState = ((UserLanguage == "fr-FR") ? LanguageStates.French : LanguageStates.English);

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
