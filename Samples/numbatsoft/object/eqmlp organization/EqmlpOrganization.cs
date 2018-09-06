using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppCSHtml5
{
    public class EqmlpOrganization : ObjectBase, IEqmlpOrganization
    {
        public EqmlpOrganization(string name, string login, string meeting, string validation)
        {
            Name = name;
            LoginLink = login;
            MeetingLink = meeting;
            ValidationLink = validation;
        }

        public string Name { get; private set; }
        public string LoginLink { get; private set; }
        public string MeetingLink { get; private set; }
        public string ValidationLink { get; private set; }

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
