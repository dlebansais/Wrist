using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class GeneratorStatePanel : GeneratorPanel, IGeneratorStatePanel
    {
        public GeneratorStatePanel(IStatePanel panel)
            : base(panel)
        {
            Index = panel.Index;
        }

        public string Index { get; private set; }
        public IGeneratorComponentIndex IndexComponent { get; private set; }

        public override bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            bool IsConnected = base.Connect(domain, components);

            if (IndexComponent == null)
            {
                foreach (IGeneratorComponent Item in components)
                    if (Item.Source.Name == Index)
                    {
                        IndexComponent = (IGeneratorComponentIndex)Item;
                        break;
                    }

                if (IndexComponent == null)
                    throw new InvalidOperationException();

                IsConnected = true;
            }

            return IsConnected;
        }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StatePanelProperties = "";
            string ElementPropertiesString = ElementProperties(currentPage, currentObject);

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<Grid{AttachedProperties(this)}{visibilityBinding}{StatePanelProperties}{ElementPropertiesString}>");

            List<string> Parameters = new List<string>();
            for (int i = 0; i < Items.Count; i++)
                Parameters.Add(i.ToString());

            IGeneratorObjectPropertyIndex IndexObjectProperty = IndexComponent.IndexObjectProperty;
            int Index = 0;
            foreach (IGeneratorLayoutElement Element in Items)
            {
                string VisibilityBinding = $" Visibility=\"{{Binding {IndexComponent.IndexObject.CSharpName}.{IndexComponent.IndexObjectProperty.CSharpName}, Converter={{StaticResource convIndexToVisibility}}, ConverterParameter={Parameters[Index++]}}}\"";
                Element.Generate(areaLayouts, design, indentation + 1, currentPage, currentObject, colorTheme, xamlWriter, VisibilityBinding);
            }

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}</Grid>");
        }
    }
}
