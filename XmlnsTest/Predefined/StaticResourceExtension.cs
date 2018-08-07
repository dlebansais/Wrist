using System;
using System.Reflection;
using System.Windows.Markup;

namespace Windows.UI.Xaml
{
    [DefaultMember("ResourceKey")]
    [MarkupExtensionReturnType(typeof(object))]
    public class StaticResourceExtension : MarkupExtension
    {
        public StaticResourceExtension(object obj) { }

        public string ResourceKey { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider) { return null; }
    }
}
