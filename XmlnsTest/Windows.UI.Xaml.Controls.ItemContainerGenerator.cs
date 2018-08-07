using System;
using System.Collections.Generic;

namespace Windows.UI.Xaml.Controls
{
    public class ItemContainerGenerator : object
    {
        public IEnumerable<object> INTERNAL_AllContainers { get; set; } = new List<object>();
    }
}
