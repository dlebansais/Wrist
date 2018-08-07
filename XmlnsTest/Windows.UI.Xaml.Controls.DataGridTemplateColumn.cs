using Windows.UI.Xaml;

namespace Windows.UI.Xaml.Controls
{
    public class DataGridTemplateColumn : DataGridBoundColumn
    {
        public DataTemplate CellTemplate { get; set; }
        public DataTemplate CellEditingTemplate { get; set; }
    }
}
