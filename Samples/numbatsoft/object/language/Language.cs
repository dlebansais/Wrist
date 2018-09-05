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
        }

        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ILogin GetLogin { get { return App.GetLogin; } }
        public IEqmlp GetEqmlp { get { return App.GetEqmlp; } }
        public INews GetNews { get { return App.GetNews; } }

        public LanguageStates LanguageState { get; set; } = LanguageStates.English;

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
