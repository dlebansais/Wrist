using System.Windows.Markup;
using Windows.UI.Xaml;
using System;

namespace Windows.UI.Xaml.Controls
{
    [ContentProperty("ContentPropertyUsefulOnlyDuringTheCompilation")]
    public class ControlTemplate : FrameworkTemplate
    {
        public Type TargetType { get; set; }
    }
}
