﻿using System.Collections.Generic;

namespace Parser
{
    public interface IObject : IForm, IConnectable
    {
        string Name { get; }
        string CSharpName { get; }
        bool IsGlobal { get; }
        IReadOnlyCollection<IObjectProperty> Properties { get; }
        IReadOnlyCollection<IObjectEvent> Events { get; }
        bool IsUsed { get; }
        void SetIsUsed();
    }
}
