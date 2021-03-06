﻿using System.Collections.Generic;

namespace Parser
{
    public interface IComponent
    {
        IDeclarationSource Source { get; }
        string XamlName { get; }
        bool IsReferencing(IArea other);
        bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject);
        bool IsUsed { get; }
        void SetIsUsed();
        void ReportResourceKeys(IDesign design, List<string> KeyList, string styleName);
    }
}
