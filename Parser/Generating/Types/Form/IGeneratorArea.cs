﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public interface IGeneratorArea : IGeneratorForm
    {
        string Name { get; }
        string XamlName { get; }
        IGeneratorObject CurrentObject { get; }
        IReadOnlyCollection<IGeneratorComponent> Components { get; }
        bool IsReferencedBy(IGeneratorArea other);
        bool Connect(IGeneratorDomain domain);
        void Generate(IGeneratorLayout layout, Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter);
        void CollectGoTo(List<Tuple<IGeneratorPageNavigation, IGeneratorObject, IGeneratorObjectPropertyBoolean>> goToList, IGeneratorPage currentPage);
        void CollectBoundComponents(List<IGeneratorBindableComponent> boundComponentList, IGeneratorPage currentPage);
    }
}
