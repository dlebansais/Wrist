using System.Windows.Markup;
using System;

namespace Windows.UI.Xaml
{
    [ContentProperty("Setters")]
    public class Style : DependencyObject
    {
        public Style BasedOn { get; set; }
        public Boolean IsSealed { get; set; }
        public SetterBaseCollection Setters { get; set; } = new SetterBaseCollection();
        public Type TargetType { get; set; }
    }
}
