using System.IO;

namespace Parser
{
    public class GeneratorComponentContainerList : GeneratorComponent, IGeneratorComponentContainerList
    {
        public GeneratorComponentContainerList(IComponentContainerList container)
            : base(container)
        {
            BaseContainer = container;
        }

        private IComponentContainerList BaseContainer;

        public IGeneratorObject ItemObject { get; private set; }
        public IGeneratorObjectPropertyItemList ItemObjectProperty { get; private set; }
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
                    ItemObjectProperty = (IGeneratorObjectPropertyItemList)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseContainer.ItemObjectProperty];
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

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (style != null) ? style : "";
            string Properties = "";
            string Value = GetComponentValue(currentPage, currentObject, null, ItemObject, ItemObjectProperty, null, false);
            string ContentBinding = $" ItemsSource=\"{Value}\"";
            string AreaTemplate = $" ItemTemplate=\"{{StaticResource {ItemNestedArea.XamlName}}}\"";

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<ItemsControl{attachedProperties}{visibilityBinding}{ContentBinding}{AreaTemplate}{Properties}{elementProperties}/>");
        }
    }
}
