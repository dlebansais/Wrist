﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Text")]
    public class TextDecoration : LayoutElement, ITextDecoration
    {
        public string Text { get; set; }
        public string Wrapping { get; set; }
        public Windows.UI.Xaml.TextWrapping? TextWrapping { get; private set; }

        public override string FriendlyName { get { return $"{GetType().Name} ({Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(Text)))})"; } }

        public override ILayoutElement GetClone()
        {
            TextDecoration Clone = new TextDecoration();
            InitializeClone(Clone);
            return Clone;
        }

        protected override void InitializeClone(LayoutElement clone)
        {
            base.InitializeClone(clone);

            ((TextDecoration)clone).Text = Text;
            ((TextDecoration)clone).Wrapping = Wrapping;
        }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (Wrapping == null)
                TextWrapping = null;
            else if (Wrapping == Windows.UI.Xaml.TextWrapping.Wrap.ToString())
                TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
            else if (Wrapping == Windows.UI.Xaml.TextWrapping.NoWrap.ToString())
                TextWrapping = Windows.UI.Xaml.TextWrapping.NoWrap;
            else
                throw new ParsingException(242, Source, $"Invalid wrapping for text decoration.");

            Dictionary<string, object> MatchTable = new Dictionary<string, object>();
            foreach (IPage Page in domain.Pages)
                MatchTable.Add(Page.Name, Page);

            List<object> MatchedList = new List<object>();
            List<string> UnmatchedList = new List<string>();
            ReplaceUri(Text, "href", '"', MatchTable, MatchedList, UnmatchedList, null);
            if (UnmatchedList.Count > 0)
                throw new ParsingException(243, Source, $"Invalid link to page '{UnmatchedList[0]}' in text decoration, page not found.");

            foreach (object Item in MatchedList)
            {
                IPage Page = (IPage)Item;
                Page.SetIsReachable();
            }
        }

        public static string ReplaceUri(string text, string uriDeclaration, char quoteChar, Dictionary<string, object> matchTable, List<object> matchedList, List<string> unmatchedList, Func<object, string> handler)
        {
            string Pattern = $"{uriDeclaration}={quoteChar}";
            int StartIndex = 0;

            while ((StartIndex = text.IndexOf(Pattern, StartIndex)) >= 0)
            {
                int EndIndex = text.IndexOf(quoteChar.ToString(), StartIndex + Pattern.Length);
                if (EndIndex > StartIndex + Pattern.Length)
                {
                    string PageName = text.Substring(StartIndex + Pattern.Length, EndIndex - StartIndex - Pattern.Length);
                    string Replacement = "";

                    foreach (KeyValuePair<string, object> Entry in matchTable)
                        if (Entry.Key == PageName)
                        {
                            if (!matchedList.Contains(Entry.Value))
                                matchedList.Add(Entry.Value);

                            if (handler != null)
                                Replacement = handler(Entry.Value);
                            else
                                Replacement = $"{uriDeclaration}=\"{PageName}\"";
                            break;
                        }

                    if (Replacement.Length > 0)
                        text = text.Substring(0, StartIndex) + Replacement + text.Substring(EndIndex + 1);
                    else
                    {
                        bool IsExternal = false;
                        int TextIndex = text.IndexOf('>', EndIndex + 1);
                        if (TextIndex > EndIndex + 1)
                        {
                            string Options = text.Substring(EndIndex + 1, TextIndex - EndIndex - 1);
                            if (Options.Contains("tag=\"external\""))
                                IsExternal = true;

                        }

                        if (!IsExternal && unmatchedList != null && !unmatchedList.Contains(PageName))
                            unmatchedList.Add(PageName);
                    }

                    StartIndex = EndIndex;
                }
            }

            return text;
        }

        public override void ReportResourceKeys(IDesign design, List<string> KeyList)
        {
            string Key = ComponentText.FormatStyleResourceKey(design.XamlName, Style);
            if (!KeyList.Contains(Key))
                KeyList.Add(Key);
        }

        public override string ToString()
        {
            return $"{GetType().Name} \"{Text}\"";
        }
    }
}
