using System;
using System.Reflection;
using System.Windows.Markup;

namespace Windows.UI.Xaml
{
    [DefaultMember("Property")]
    [MarkupExtensionReturnType(typeof(object))]
    public class TemplateBindingExtension : MarkupExtension
    {
        public TemplateBindingExtension(object obj) { }

        public object Converter { get; set; }
        public object ConverterParameter { get; set; }
        public object Property { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider) { return null; }
    }
}
