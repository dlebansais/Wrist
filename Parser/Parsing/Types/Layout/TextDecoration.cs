using System.Collections.Generic;

namespace Parser
{
    public class TextDecoration : LayoutElement, ITextDecoration
    {
        public string Text { get; set; }
        public string Wrapping { get; set; }
        public Windows.UI.Xaml.TextWrapping? TextWrapping { get; private set; }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (Wrapping == null)
                TextWrapping = null;
            else if (Wrapping == Windows.UI.Xaml.TextWrapping.Wrap.ToString())
                TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
            else if (Wrapping == Windows.UI.Xaml.TextWrapping.NoWrap.ToString())
                TextWrapping = Windows.UI.Xaml.TextWrapping.NoWrap;
            else
                throw new ParsingException(0, Source, $"Invalid wrapping for text decoration.");
        }

        public override string ToString()
        {
            return $"{GetType().Name} \"{Text}\"";
        }
    }
}
