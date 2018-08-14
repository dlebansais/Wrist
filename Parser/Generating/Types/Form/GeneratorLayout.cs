using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorLayout : IGeneratorLayout
    {
        public static Dictionary<ILayout, IGeneratorLayout> GeneratorLayoutMap { get; } = new Dictionary<ILayout, IGeneratorLayout>();

        public GeneratorLayout(ILayout layout)
        {
            Name = layout.Name;
            XamlName = layout.XamlName;
            FileName = layout.FileName;
            LayoutBase = layout;

            GeneratorLayoutMap.Add(layout, this);
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public string FileName { get; private set; }
        public Dictionary<IGeneratorArea, IGeneratorPanel> ContentTable { get; } = new Dictionary<IGeneratorArea, IGeneratorPanel>();
        private ILayout LayoutBase;

        public bool Connect(IGeneratorDomain domain, IGeneratorArea area)
        {
            bool IsConnected = false;

            IGeneratorPanel Content;
            if (!ContentTable.ContainsKey(area))
            {
                IsConnected = true;
                Content = (IGeneratorPanel)GeneratorPanel.Convert(LayoutBase.Content);

                ContentTable.Add(area, Content);
            }
            else
                Content = ContentTable[area];

            IsConnected |= Content.Connect(domain, area.Components);

            return IsConnected;
        }

        public void Generate(IGeneratorArea area, Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter)
        {
            IGeneratorPanel Content = ContentTable[area];
            Content.Generate(areaLayouts, design, indentation, currentPage, currentObject, colorTheme, xamlWriter, "");
        }

        public static string IndentationString(int indentation)
        {
            string Result = "";
            for (int i = 0; i < indentation; i++)
                Result += "    ";

            return Result;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
