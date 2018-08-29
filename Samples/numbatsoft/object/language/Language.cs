﻿using DatabaseManager;
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
    public enum LanguageStates
    {
        English,
        French,
    }

    public class Language : ILanguage
    {
        public Language()
        {
            LanguageState = ((Persistent.GetValue("language", "english") == "french") ? LanguageStates.French : LanguageStates.English);

            InitSimulation();
        }

        public ILanguage GetLanguage { get { return App.GetLanguage; } }
        public ILogin GetLogin { get { return App.GetLogin; } }

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

        public void On_Switch(string pageName, string sourceName, string sourceContent)
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

        private void OnNewsReceived(bool success, object result)
        {
            if (!success)
                return;

            List<Dictionary<string, string>> NewsList = (List<Dictionary<string, string>>)result;
            Debug.WriteLine($"{NewsList.Count} news entries received");

            foreach (Dictionary<string, string> Item in NewsList)
            {
                NewsEntry NewEntry = new NewsEntry(LanguageState, Item["enu_summary"], Item["enu_content"], Item["fra_summary"], Item["fra_content"]);
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
                { "enu_summary", "summary 0" },
                { "enu_content", "content 0" },
                { "fra_summary", "sommaire 0" },
                { "fra_content", "contenu 0" },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "enu_summary", "summary 1" },
                { "enu_content", "content 1" },
                { "fra_summary", "sommaire 1" },
                { "fra_content", "contenu 1" },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "enu_summary", "summary 2" },
                { "enu_content", "content 2" },
                { "fra_summary", "sommaire 2" },
                { "fra_content", "contenu 2" },
            });
            Result.Add(new Dictionary<string, string>()
            {
                { "enu_summary", "summary 3" },
                { "enu_content", "content 3" },
                { "fra_summary", "sommaire 3" },
                { "fra_content", "contenu 3" },
            });

            return Result;
        }
        #endregion

        public static string ReplaceHtml(string text)
        {
            string Result = text;
            Result = Result.Replace("&nbsp;", "\u00A0");

            return Result;
        }

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
