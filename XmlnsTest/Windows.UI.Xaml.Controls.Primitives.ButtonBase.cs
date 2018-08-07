using System.Windows.Markup;
using Windows.UI.Xaml.Controls;
using System;
using System.Windows.Input;

namespace Windows.UI.Xaml.Controls.Primitives
{
    [ContentProperty("Content")]
    public class ButtonBase : ContentControl
    {
        public object CommandParameter { get; set; }
        public ICommand Command { get; set; }
        public Boolean IsPressed { get; set; }
        public ClickMode ClickMode { get; set; }
    }
}
