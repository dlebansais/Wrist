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
            "#sc_root		{border-style:solid; border-width:3px; border-color:#00828F; border-radius:5px; font-family:Consolas; padding:10px 10px 0 10px; margin-bottom:10px; margin-left:10px; margin-right:10px; text-align:left; }\n" +
            "#sc_root_floating	{border-style:solid; border-width:3px; border-color:#00828F; border-radius:5px; font-family:Consolas; padding:10px 10px 0 10px; margin-bottom:10px; margin-left:10px; margin-right:10px; float:left; width:390px; min-height:310px; text-align:left; }\n" +
            "#sc_root p		{margin:0; padding:0 0 2px 0; }\n" +
            "#sc_root_floating p	{margin:0; padding:0 0 2px 0; }\n" +
            "#sc_legend		{font-family:\"Trebuchet MS\", verdana, arial, \"Times New Roman\", serif; padding-bottom:20px; float:right; }\n" +
            "#sc_legend_floating	{font-family:\"Trebuchet MS\", verdana, arial, \"Times New Roman\", serif; padding-bottom:20px; text-align:center; }\n" +
            "#sc_neutral		{color:#000000; }\n" +
            "#sc_keyword		{color:#0000FF; }\n" +
            "#sc_type		{color:#2B91AF; }\n" +
            "#sc_char		{color:#FFA500; }\n" +
            "#sc_string		{color:#FFA500; }\n" +
            "#sc_number_black	{color:#000000; }\n" +
            "#sc_number_blue		{color:#0000FF; }\n" +
            "#sc_number_green	{color:#008000; }\n" +
            "#sc_enum		{color:#8B0000; }\n" +
            "#sc_tab			{padding-right:16px; }\n" +
            "</style>\n";


        public SampleCode()
        {
            TitleTable.Add(LanguageStates.English, null);
            TitleTable.Add(LanguageStates.French, null);
        }

        public bool IsFrontPage { get; private set; }
        public string Content { get; private set; }
        public string Feature { get; private set; }
        protected LanguageStates LanguageState { get { return GetLanguage.LanguageState; } }
        public string Title { get { return TitleTable[LanguageState]; } }

        public void UpdateContent(bool isFrontPage, string feature, string content, string titleEnu, string titleFra)
        {
            IsFrontPage = isFrontPage;
            Content = $"{ContentStyle}\n{content}";
            Feature = feature;

            TitleTable[LanguageStates.English] = titleEnu;
            TitleTable[LanguageStates.French] = titleFra;

            NotifyPropertyChanged(nameof(IsFrontPage));
            NotifyPropertyChanged(nameof(Content));
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
