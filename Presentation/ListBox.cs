using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Presentation
{
    public class ListBox : Windows.UI.Xaml.Controls.ListBox
    {
        #region Custom properties and events
        #region ControlSelectedIndex
        /// <summary>
        ///     Identifies the <see cref="ControlSelectedIndex"/> dependency property.
        /// </summary>
        /// <returns>
        ///     The identifier for the <see cref="ControlSelectedIndex"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ControlSelectedIndexProperty = DependencyProperty.Register("ControlSelectedIndex", typeof(int), typeof(ListBox), new PropertyMetadata(-1, new PropertyChangedCallback(OnControlSelectedIndexPropertyChanged)));

        /// <summary>
        ///     Gets or sets the ControlSelectedIndex property.
        /// </summary>
        public int ControlSelectedIndex
        {
            get { return (int)GetValue(ControlSelectedIndexProperty); }
            set { SetValue(ControlSelectedIndexProperty, value); }
        }

        private static void OnControlSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox ctrl = (ListBox)d;
            ctrl.OnControlSelectedIndexPropertyChanged(e);
        }

        private void OnControlSelectedIndexPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedIndex != ControlSelectedIndex)
            {
                SelectedIndex = ControlSelectedIndex;
                SetSelectedItem(SelectedIndex);
            }
        }
        #endregion
        #endregion

        public ListBox()
        {
            SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ControlSelectedIndex != SelectedIndex)
            {
                SetValue(ControlSelectedIndexProperty, SelectedIndex);
                SetSelectedItem(SelectedIndex);
            }
        }

        private void SetSelectedItem(int index)
        {
            /*
            for (int i = 0; i < Items.Count; i++)
            {
                ISelectorSelection AsListBoxSelection;
                if ((AsListBoxSelection = Items[i] as ISelectorSelection) != null)
                    AsListBoxSelection.IsSelected = (i == index);
            }*/
        }
    }
}