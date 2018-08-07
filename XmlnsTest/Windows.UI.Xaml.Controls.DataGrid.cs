using System.Windows.Markup;
using Windows.UI.Xaml.Controls.Primitives;
using System;
using Windows.UI.Xaml;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("Items")]
    public class DataGrid : MultiSelector
    {
        public Boolean AutoGenerateColumns { get; set; }
        public Style ColumnHeaderStyle { get; set; }
        public ObservableCollection<DataGridColumn> Columns { get; set; } = new ObservableCollection<DataGridColumn>();
        public Boolean IsReadOnly { get; set; }
        public DataGridSelectionMode SelectionMode { get; set; }
        public Brush HorizontalGridLinesBrush { get; set; }
        public DataTemplate RowHeaderTemplate { get; set; }
        public Brush VerticalGridLinesBrush { get; set; }
    }
}
