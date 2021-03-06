﻿using System.Collections.Generic;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class StatePanel : Panel, IStatePanel
    {
        public string Index { get; set; }
        public IComponentIndex Component { get; private set; }

        public override ILayoutElement GetClone()
        {
            StatePanel Clone = new StatePanel();
            InitializeClone(Clone);
            return Clone;
        }

        protected override void InitializeClone(LayoutElement clone)
        {
            base.InitializeClone(clone);

            ((StatePanel)clone).Index = Index;
        }

        public override void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            base.ConnectComponents(domain, currentDynamic, components);

            if (Index == null)
                throw new ParsingException(200, Source, $"StatePanel has no index.");

            IComponent FoundComponent = null;
            foreach (IComponent Item in components)
                if (Item.Source.Name == Index)
                {
                    FoundComponent = Item;
                    break;
                }

            if (FoundComponent is IComponentIndex AsIndexComponent)
            {
                IObjectPropertyIndex IndexObjectProperty = AsIndexComponent.IndexObjectProperty;

                if (IndexObjectProperty is IObjectPropertyBoolean AsBooleanProperty)
                {
                    if (Items.Count != 2)
                        throw new ParsingException(201, Source, $"StatePanel is referencing '{Index}' but doesn't have the two items expected for a boolean property.");
                }
                else if (Items.Count < 2)
                    throw new ParsingException(202, Source, $"StatePanel must have at least two items.");

                Component = AsIndexComponent;
                Component.SetIsUsed();
            }

            else if (FoundComponent == null)
                throw new ParsingException(171, Source, $"StatePanel is referencing '{Index}' but this index doesn't exist.");

            else
                throw new ParsingException(172, Source, $"StatePanel is referencing '{Index}' but this component is not an index.");
        }

        public override string ToString()
        {
            return $"{GetType().Name} with index '{Index}'";
        }
    }
}
