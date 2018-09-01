using System.Collections.Generic;
using System.IO;
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
            BasePanel = panel;
        }

        private IStatePanel BasePanel;

        public string Index { get; private set; }
        public IGeneratorComponentIndex Component { get; private set; }

        public override bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            bool IsConnected = base.Connect(domain, components);

            if (Component == null)
            {
                if (GeneratorComponent.GeneratorComponentMap.ContainsKey(BasePanel.Component))
                    Component = (IGeneratorComponentIndex)GeneratorComponent.GeneratorComponentMap[BasePanel.Component];

                IsConnected = true;
            }

            return IsConnected;
        }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string AttachedProperties = GetAttachedProperties();
            string StatePanelProperties = GetPanelProperties(currentPage, currentObject);
            string ElementProperties = GetElementProperties(currentPage, currentObject);

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<Grid{AttachedProperties}{visibilityBinding}{StatePanelProperties}{ElementProperties}>");

            List<string> Parameters = new List<string>();
            for (int i = 0; i < Items.Count; i++)
                Parameters.Add(i.ToString());

            IGeneratorObjectPropertyIndex IndexObjectProperty = Component.IndexObjectProperty;
            int Index = 0;
            foreach (IGeneratorLayoutElement Element in Items)
            {
                string IndexReference = Component.GetObjectBinding(currentObject, Component.IndexObject, Component.IndexObjectProperty);
                string VisibilityBinding = $" Visibility=\"{{Binding {IndexReference}, Converter={{StaticResource convIndexToVisibility}}, ConverterParameter={Parameters[Index++]}}}\"";
                Element.Generate(areaLayouts, pageList, design, indentation + 1, currentPage, currentObject, colorTheme, xamlWriter, VisibilityBinding);
            }

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}</Grid>");
        }
    }
}
