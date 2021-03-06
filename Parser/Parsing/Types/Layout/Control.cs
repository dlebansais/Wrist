﻿using System.Collections.Generic;

namespace Parser
{
    public class Control : LayoutElement, IControl
    {
        public string Name { get; set; }
        public string Wrapping { get; set; }
        public Windows.UI.Xaml.TextWrapping? TextWrapping { get; private set; }
        public IComponent Component { get; private set; }

        public override string FriendlyName { get { return Name; } }

        public override ILayoutElement GetClone()
        {
            Control Clone = new Control();
            InitializeClone(Clone);
            return Clone;
        }

        protected override void InitializeClone(LayoutElement clone)
        {
            base.InitializeClone(clone);

            ((Control)clone).Name = Name;
            ((Control)clone).Wrapping = Wrapping;
        }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (string.IsNullOrEmpty(Name))
                throw new ParsingException(218, Source, $"Control must reference a name.");

            if (Component == null)
            {
                foreach (IComponent Item in components)
                    if (Item.Source.Name == Name)
                    {
                        Component = Item;
                        break;
                    }

                if (Component == null)
                    throw new ParsingException(156, Source, $"Control is referencing '{Name}' but this name doesn't exist.");

                Component.SetIsUsed();
            }

            if (Wrapping == null)
                TextWrapping = null;
            else if (Wrapping == Windows.UI.Xaml.TextWrapping.Wrap.ToString())
                TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
            else if (Wrapping == Windows.UI.Xaml.TextWrapping.NoWrap.ToString())
                TextWrapping = Windows.UI.Xaml.TextWrapping.NoWrap;
            else
                throw new ParsingException(157, Source, $"Invalid wrapping for '{Name}'.");
        }

        public override void ReportResourceKeys(IDesign design, List<string> KeyList)
        {
            Component.ReportResourceKeys(design, KeyList, Style);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
