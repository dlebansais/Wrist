﻿using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public interface IGeneratorDynamicProperty
    {
        IDeclarationSource Source { get; }
        string CSharpName { get; }
        DynamicOperationResults Result { get; }
        IGeneratorDynamicOperation RootOperation { get; }
        bool Connect(IGeneratorDomain domain);
        void GetUsedObjects(Dictionary<IGeneratorObject, List<IGeneratorObjectProperty>> usedObjectTable);
        void Generate(StreamWriter cSharpWriter);
        void GenerateNotification(IGeneratorObject obj, IGeneratorObjectProperty objectProperty, string xamlPageName, StreamWriter cSharpWriter);
    }
}
