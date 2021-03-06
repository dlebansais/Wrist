﻿using System.Collections.Generic;

namespace Parser
{
    public abstract class Component : IComponent
    {
        public Component(IDeclarationSource source, string xamlName)
        {
            Source = source;
            XamlName = xamlName;
        }

        public IDeclarationSource Source { get; private set; }
        public string XamlName { get; private set; }
        public bool IsUsed { get; private set; }

        public abstract bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject);

        public virtual void ReportResourceKeys(IDesign design, List<string> KeyList, string styleName)
        {
        }

        public void SetIsUsed()
        {
            IsUsed = true;
        }

        public virtual bool IsReferencing(IArea other)
        {
            return false;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Source.Name}'";
        }
    }
}
