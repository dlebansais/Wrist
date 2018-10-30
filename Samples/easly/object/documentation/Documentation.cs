using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class Documentation : ObjectBase, IDocumentation
    {
        public Documentation()
        {
        }

        public string IntroductionLink { get { return $"https://www.easly.org/documentation/an_introduction_to_easly.pdf"; } }
        public string EditorManualLink { get { return $"https://www.easly.org/documentation/editor_manual.pdf"; } }
        public string EditorSpecificationsLink { get { return $"https://www.easly.org/documentation/specifications_of_the_easly_source_code_editor.pdf"; } }
        public string SolutionManagerSpecificationsLink { get { return $"https://www.easly.org/documentation/specifications_of_the_easly_solution_manager.pdf"; } }
        public string ReferenceLink { get { return $"https://www.easly.org/documentation/easly_the_programming_language_reference.pdf"; } }
        public string DesignLink { get { return $"https://www.easly.org/documentation/easly_design_decisions.pdf"; } }

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
