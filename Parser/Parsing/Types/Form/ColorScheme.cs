﻿using System.Collections.Generic;

namespace Parser
{
    public class ColorScheme : IColorScheme, IConnectable
    {
        public ColorScheme(string name, Dictionary<IDeclarationSource, string> colors)
        {
            Name = name;
            Colors = colors;
        }

        public string Name { get; private set; }
        public Dictionary<IDeclarationSource, string> Colors { get; private set; }

        public bool Connect(IDomain domain)
        {
            return false;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
