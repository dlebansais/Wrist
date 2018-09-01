using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentContainer : GeneratorComponent, IGeneratorComponentContainer
    {
        public GeneratorComponentContainer(IComponentContainer container)
            : base(container)
        {
            BaseContainer = container;
        }

        private IComponentContainer BaseContainer;

        public IGeneratorObject ItemObject { get; private set; }
        public IGeneratorObjectPropertyItem ItemObjectProperty { get; private set; }
        public IGeneratorObject ItemNestedObject { get; private set; }
        public IGeneratorArea ItemNestedArea { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (ItemObject == null || ItemObjectProperty == null)
            {
                IsConnected = true;
                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseContainer.ItemObject))
                    ItemObject = GeneratorObject.GeneratorObjectMap[BaseContainer.ItemObject];
                if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseContainer.ItemObjectProperty))
                    ItemObjectProperty = (IGeneratorObjectPropertyItem)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseContainer.ItemObjectProperty];
                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseContainer.ItemNestedObject))
                    ItemNestedObject = GeneratorObject.GeneratorObjectMap[BaseContainer.ItemNestedObject];
                if (GeneratorArea.GeneratorAreaMap.ContainsKey(BaseContainer.ItemNestedArea))
                    ItemNestedArea = GeneratorArea.GeneratorAreaMap[BaseContainer.ItemNestedArea];
            }

            return IsConnected;
        }

        public override bool IsReferencing(IGeneratorArea other)
        {
            if (ItemNestedArea == other)
                return true;

            else if (other.IsReferencedBy(ItemNestedArea))
                return true;

            else
                return false;
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string Properties = "";
            string Value = GetComponentValue(currentPage, currentObject, null, ItemObject, ItemObjectProperty, null, false);
            string ContentBinding = $" Content=\"{Value}\"";
            string AreaTemplate = $" ContentTemplate=\"{{StaticResource {ItemNestedArea.XamlName}}}\"";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<ContentControl{attachedProperties}{visibilityBinding}{ContentBinding}{AreaTemplate}{Properties}{elementProperties}/>");
        }
    }
}
