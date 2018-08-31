using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorControl : GeneratorLayoutElement, IGeneratorControl
    {
        public GeneratorControl(Control control)
            : base(control)
        {
            Name = control.Name;
            TextWrapping = control.TextWrapping;

            BaseControl = control;
        }

        private IControl BaseControl;

        public string Name { get; private set; }
        public Windows.UI.Xaml.TextWrapping? TextWrapping { get; private set; }
        public IGeneratorComponent Component { get; private set; }

        public override bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            bool IsConnected = base.Connect(domain, components);

            if (Component == null)
            {
                IsConnected = true;

                if (GeneratorComponent.GeneratorComponentMap.ContainsKey(BaseControl.Component))
                    Component = GeneratorComponent.GeneratorComponentMap[BaseControl.Component];
            }

            return IsConnected;
        }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            bool IsHorizontalAlignmentStretch = (HorizontalAlignment == Windows.UI.Xaml.HorizontalAlignment.Stretch.ToString());
            string AttachedProperties = GetAttachedProperties();
            string ElementProperties = GetElementProperties(currentPage, currentObject);

            Component.Generate(design, Style, AttachedProperties, ElementProperties, TextWrapping, IsHorizontalAlignmentStretch, indentation, currentPage, currentObject, colorTheme, xamlWriter, visibilityBinding);
        }

        public override string GetStyleResourceKey(IGeneratorDesign design)
        {
            return Component.GetStyleResourceKey(design, Style);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
