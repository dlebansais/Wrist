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
        public IGeneratorPanel Content{ get; private set; }
        private ILayout LayoutBase;

        public bool Connect(IGeneratorDomain domain, IGeneratorArea area)
        {
            bool IsConnected = false;

            if (Content == null)
            {
                IsConnected = true;
                Content = GeneratorPanel.Convert(LayoutBase.Content);
            }

            IsConnected |= Content.Connect(domain, area.Components);

            return IsConnected;
        }

        public void Generate(IGeneratorArea area, Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter)
        {
            Content.Generate(areaLayouts, pageList, design, indentation, currentPage, currentObject, colorTheme, xamlWriter, "");
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
