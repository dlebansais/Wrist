using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class SampleCode : ObjectBase, ISampleCode
    {
        private static readonly string ContentStyle =
            "<style type='text/css'>\n" +
            "body                {background:#FFFFFF; color:#000000; font-family: Arial, \"Trebuchet MS\", Verdana, \"Times New Roman\", Serif; font-size:90%; }\n" +
            "p                   {margin:0; padding:0 0 15px 0; }\n" +
            "li                  {margin:0; padding:0 0 5px 0; }\n" +
            "a                   {color:#000000; text-decoration:underline; }\n" +
            "a:hover             {color:#B23201; text-decoration:none;}\n" +
            "\n" +
            "#sc_root		     {border-style:solid; border-width:3px; border-color:#00828F; border-radius:5px; font-family:Consolas; padding:10px 10px 10px 10px; margin-bottom:10px; margin-left:10px; margin-right:10px; text-align:left; }\n" +
            "#sc_root_floating   {border-style:solid; border-width:3px; border-color:#00828F; border-radius:5px; font-family:Consolas; padding:40px 10px 10px 10px; margin-bottom:10px; margin-left:10px; margin-right:10px; float:left; width:390px; min-height:310px; text-align:left; }\n" +
            "#sc_root p          {margin:0; padding:0 0 2px 0; }\n" +
            "#sc_root_floating p {margin:0; padding:0 0 2px 0; }\n" +
            "#sc_legend          {font-family:\"Trebuchet MS\", verdana, arial, \"Times New Roman\", serif; padding-bottom:20px; float:right; }\n" +
            "#sc_legend_floating {font-family:\"Trebuchet MS\", verdana, arial, \"Times New Roman\", serif; padding-bottom:20px; text-align:center; }\n" +
            "#sc_neutral         {color:#000000; }\n" +
            "#sc_keyword         {color:#0000FF; }\n" +
            "#sc_type            {color:#2B91AF; }\n" +
            "#sc_char            {color:#FFA500; }\n" +
            "#sc_string          {color:#FFA500; }\n" +
            "#sc_number_black    {color:#000000; }\n" +
            "#sc_number_blue     {color:#0000FF; }\n" +
            "#sc_number_green    {color:#008000; }\n" +
            "#sc_enum            {color:#8B0000; }\n" +
            "#sc_tab             {padding-right:16px; }\n" +
            "</style>\n";

        private static readonly string ContentHeaderNormal = "<div id=\"sc_root\">\n";
        private static readonly string ContentHeaderFloating = "<div id=\"sc_root_floating\">\n";
        private static readonly string ContentFooter = "</div>\n";

        public SampleCode(PageNames pageName, bool isFrontPage)
        {
            PageName = pageName;
            IsFrontPage = isFrontPage;
            IsLoaded = false;
            TitleTable.Add(LanguageStates.English, null);
            TitleTable.Add(LanguageStates.French, null);
        }

        public PageNames PageName { get; private set; }
        public bool IsFrontPage { get; private set; }
        public bool IsLoaded { get; private set; }
        public string ContentNormal { get { return $"{ContentStyle}\n{ContentHeaderNormal}{Text}{ContentFooter}"; } }
        public string ContentFloating { get { return $"{ContentStyle}\n{ContentHeaderFloating}{Text}{ContentFooter}"; } }
        private string Text;
        public string Feature { get; private set; }
        protected LanguageStates LanguageState { get { return GetLanguage.LanguageState; } }
        public string Title { get { return TitleTable[LanguageState]; } }

        public void UpdateContent(string feature, string text, string titleEnu, string titleFra)
        {
            IsLoaded = true;
            Text = text;
            Feature = feature;

            TitleTable[LanguageStates.English] = titleEnu;
            TitleTable[LanguageStates.French] = titleFra;

            NotifyPropertyChanged(nameof(IsFrontPage));
            NotifyPropertyChanged(nameof(ContentNormal));
            NotifyPropertyChanged(nameof(ContentFloating));
            NotifyPropertyChanged(nameof(Feature));
            NotifyPropertyChanged(nameof(Title));
        }

        public Dictionary<LanguageStates, string> TitleTable { get; } = new Dictionary<LanguageStates, string>();

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
