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
    public class Language : ILanguage, INotifyPropertyChanged
    {
        public Language()
        {
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