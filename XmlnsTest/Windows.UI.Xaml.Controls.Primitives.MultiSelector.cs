using System.Windows.Markup;
using System.Collections;
using System.Collections.Generic;

namespace Windows.UI.Xaml.Controls.Primitives
{
    [ContentProperty("Items")]
    public class MultiSelector : Selector
    {
        public IList SelectedItems { get; set; } = new List<object>();
    }
}
