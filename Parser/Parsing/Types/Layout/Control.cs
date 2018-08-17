using System.Collections.Generic;

namespace Parser
{
    public class Control : LayoutElement, IControl
    {
        public string Name { get; set; }
        public string Wrapping { get; set; }
        public Windows.UI.Xaml.TextWrapping? TextWrapping { get; private set; }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            bool IsFound = false;
            foreach (IComponent Component in components)
                if (Component.Source.Name == Name)
                {
                    IsFound = true;
                    break;
                }

            if (!IsFound)
                throw new ParsingException(156, Source, $"Control is referencing '{Name}' but this name doesn't exist.");

            if (Wrapping == null)
                TextWrapping = null;
            else if (Wrapping == Windows.UI.Xaml.TextWrapping.Wrap.ToString())
                TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
            else if (Wrapping == Windows.UI.Xaml.TextWrapping.NoWrap.ToString())
                TextWrapping = Windows.UI.Xaml.TextWrapping.NoWrap;
            else
                throw new ParsingException(157, Source, $"Invalid wrapping for '{Name}'.");
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
