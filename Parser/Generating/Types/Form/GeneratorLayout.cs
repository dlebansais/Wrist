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
            Content = (IGeneratorPanel)GeneratorPanel.Convert(layout.Content);

            GeneratorLayoutMap.Add(layout, this);
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public IGeneratorPanel Content { get; private set; }

        public bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            return Content.Connect(domain, components);
        }

        public void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter)
        {
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
