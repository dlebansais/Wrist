using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Parser
{
    public class GeneratorTextDecoration : GeneratorLayoutElement, IGeneratorTextDecoration
    {
        public GeneratorTextDecoration(TextDecoration control)
            : base(control)
        {
            Text = control.Text;
            TextWrapping = control.TextWrapping;
        }

        public string Text { get; private set; }
        public Windows.UI.Xaml.TextWrapping? TextWrapping { get; private set; }
        public Dictionary<IGeneratorPage, string> LinkedPageTable { get; } = new Dictionary<IGeneratorPage, string>();

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string AttachedProperties = GetAttachedProperties();
            string StyleProperty = (Style != null) ? Style : "";
            string WrappingProperty = ((TextWrapping.HasValue && TextWrapping.Value == Windows.UI.Xaml.TextWrapping.NoWrap) ? " TextWrapping=\"NoWrap\"" : " TextWrapping=\"Wrap\"");
            string Properties = $" Style=\"{{StaticResource {design.XamlName}Text{StyleProperty}}}\"";
            string ElementProperties = GetElementProperties(currentPage, currentObject);

            /*
            Debug.Assert(TextToSpan("test") == "test");

            Debug.Assert(TextToSpan("<i/>") == "");
            Debug.Assert(TextToSpan("<i></i>") == "");
            Debug.Assert(TextToSpan("<i>test</i>") == "<Italic>test</Italic>");
            Debug.Assert(TextToSpan("x<i>test</i>") == "x<Italic>test</Italic>");
            Debug.Assert(TextToSpan("<i>test</i>x") == "<Italic>test</Italic>x");
            Debug.Assert(TextToSpan("x<i>test</i>x") == "x<Italic>test</Italic>x");
            Debug.Assert(TextToSpan("<i>test") == "<Italic>test</Italic>");
            Debug.Assert(TextToSpan("x<i>test") == "x<Italic>test</Italic>");
            Debug.Assert(TextToSpan("<i>test</i><i>test</i>") == "<Italic>test</Italic><Italic>test</Italic>");
            Debug.Assert(TextToSpan("<i>test</i>x<i>test</i>") == "<Italic>test</Italic>x<Italic>test</Italic>");
            Debug.Assert(TextToSpan("<i>test</i>x<i>test</i>x") == "<Italic>test</Italic>x<Italic>test</Italic>x");
            Debug.Assert(TextToSpan("<i>test</i><i>test") == "<Italic>test</Italic><Italic>test</Italic>");
            Debug.Assert(TextToSpan("<i>test</i>x<i>test") == "<Italic>test</Italic>x<Italic>test</Italic>");
            Debug.Assert(TextToSpan("x<i>test</i><i>test") == "x<Italic>test</Italic><Italic>test</Italic>");
            Debug.Assert(TextToSpan("x<i>test</i>x<i>test") == "x<Italic>test</Italic>x<Italic>test</Italic>");

            Debug.Assert(TextToSpan("<b>test</b>") == "<Bold>test</Bold>");
            Debug.Assert(TextToSpan("<u>test</u>") == "<Underline>test</Underline>");
            Debug.Assert(TextToSpan("<p/>test") == "<LineBreak/>test");
            Debug.Assert(TextToSpan("<z>test</z>") == "<z>test</z>");

            Debug.Assert(TextToSpan("<font>test</font>") == "<Span>test</Span>");
            Debug.Assert(TextToSpan("<font size=\"20\">test</font>") == "<Span FontSize=\"20\">test</Span>");
            Debug.Assert(TextToSpan("<font color=\"Red\">test</font>") == "<Span Foreground=\"Red\">test</Span>");
            Debug.Assert(TextToSpan("<font background=\"Red\">test</font>") == "<Span Background=\"Red\">test</Span>");
            Debug.Assert(TextToSpan("<font face=\"Verdana\">test</font>") == "<Span FontFamily=\"Verdana\">test</Span>");
            Debug.Assert(TextToSpan("<font face=\"Verdana\" size=\"20\" background=\"Red\" color=\"Red\">test</font>") == "<Span FontFamily=\"Verdana\" FontSize=\"20\" Background=\"Red\" Foreground=\"Red\">test</Span>");

            Debug.Assert(TextToSpan("<font size=\"20\">te<i>xx</i><b>st</b></font>") == "<Span FontSize=\"20\">te<Italic>xx</Italic><Bold>st</Bold></Span>");
            */

            string SpanText = TextToSpan(Text, pageList);
            if (SpanText == Text)
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<TextBlock{AttachedProperties}{visibilityBinding} Text=\"{SpanText}\"{Properties}{ElementProperties}{WrappingProperty}/>");
            else
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<TextBlock{AttachedProperties}{visibilityBinding}{Properties}{ElementProperties}{WrappingProperty}>{SpanText}</TextBlock>");
        }

        private string TextToSpan(string text, IList<IGeneratorPage> pageList)
        {
            text = ReplaceTag(text, pageList, "i", "Italic", false);
            text = ReplaceTag(text, pageList, "b", "Bold", false);
            text = ReplaceTag(text, pageList, "u", "Underline", false);
            text = ReplaceTag(text, pageList, "p", "LineBreak", true);
            text = ReplaceTag(text, pageList, "font", "Span", false, new Dictionary<string, string>() { { "size", "FontSize" }, { "color", "Foreground" }, { "background", "Background" }, { "face", "FontFamily" } });
            text = ReplaceTag(text, pageList, "a", "Hyperlink", false, new Dictionary<string, string>() { { "href", "NavigateUri" } });

            text = ReplaceUri(text, pageList);

            return text;
        }

        private string ReplaceTag(string text, IList<IGeneratorPage> pageList, string textTag, string spanTag, bool keepIfEmpty)
        {
            return ReplaceTag(text, pageList, textTag, spanTag, keepIfEmpty, new Dictionary<string, string>());
        }

        private string ReplaceTag(string text, IList<IGeneratorPage> pageList, string textTag, string spanTag, bool keepIfEmpty, Dictionary<string, string> tagOptions)
        {
            int LastIndex = 0;
            string Result = "";

            int i;
            for (i = 0; i < text.Length; i++)
                if (text[i] == '<')
                {
                    Result += text.Substring(LastIndex, i - LastIndex);

                    if (text.Length > i + textTag.Length && text.Substring(i + 1, textTag.Length) == textTag)
                    {
                        int j = i + 1 + textTag.Length;
                        while (j < text.Length && text[j] != '>')
                            j++;

                        string InsideTag;
                        string Options;
                        if (text.Substring(j - 1, 2) == "/>")
                        {
                            Options = text.Substring(i + textTag.Length + 1, j - textTag.Length - i - 2).Trim();
                            InsideTag = "";
                            LastIndex = j + 1;
                        }
                        else
                        {
                            Options = text.Substring(i + textTag.Length + 1, j - textTag.Length - i - 1).Trim();

                            string EndPattern = $"</{textTag}>";

                            int EndIndex = text.IndexOf(EndPattern, j + 1);
                            if (EndIndex > j)
                            {
                                InsideTag = text.Substring(j + 1, EndIndex - j - 1);
                                LastIndex = EndIndex + EndPattern.Length;
                            }
                            else
                            {
                                InsideTag = text.Substring(j + 1);
                                LastIndex = text.Length;
                            }
                        }

                        foreach (KeyValuePair<string, string> Entry in tagOptions)
                            Options = Options.Replace(Entry.Key, Entry.Value);
                        if (Options.Length > 0)
                            Options = " " + Options;

                        InsideTag = TextToSpan(InsideTag, pageList);

                        if (InsideTag.Length > 0)
                            Result += $"<{spanTag}{Options}>" + InsideTag + $"</{spanTag}>";
                        else if (keepIfEmpty)
                            Result += $"<{spanTag}{Options}/>";

                        i = LastIndex - 1;
                    }
                    else
                        LastIndex = i;
                }

            Result += text.Substring(LastIndex, i - LastIndex);
            return Result;
        }

        private string ReplaceUri(string text, IList<IGeneratorPage> pageList)
        {
            LinkedPageTable.Clear();

            string Pattern = "NavigateUri=\"";
            int StartIndex = 0;

            while ((StartIndex = text.IndexOf(Pattern, StartIndex)) >= 0)
            {
                int EndIndex = text.IndexOf("\"", StartIndex + Pattern.Length);
                if (EndIndex > StartIndex + Pattern.Length)
                {
                    string PageName = text.Substring(StartIndex + Pattern.Length, EndIndex - StartIndex - Pattern.Length);
                    string ClickEvent = "";

                    foreach (IGeneratorPage Page in pageList)
                        if (Page.Name == PageName)
                        {
                            if (!LinkedPageTable.ContainsKey(Page))
                                LinkedPageTable.Add(Page, $"OnLinkClick_{Page.XamlName}");

                            ClickEvent = $" Click=\"{LinkedPageTable[Page]}\"";
                            break;
                        }

                    if (ClickEvent.Length > 0)
                        text = text.Substring(0, StartIndex) + ClickEvent + text.Substring(EndIndex + 1);
                    else
                        StartIndex = EndIndex;
                }
            }

            return text;
        }

        public override string ToString()
        {
            return $"{GetType().Name} \"{Text}\"";
        }
    }
}
