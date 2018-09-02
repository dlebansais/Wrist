using System;
using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentSelector : GeneratorComponent, IGeneratorComponentSelector
    {
        public GeneratorComponentSelector(IComponentSelector selector)
            : base(selector)
        {
            BaseSelector = selector;
        }

        IComponentSelector BaseSelector;

        public IGeneratorObject IndexObject { get; private set; }
        public IGeneratorObjectPropertyIndex IndexObjectProperty { get; private set; }
        public IGeneratorResource ItemsResource { get; private set; }
        public IGeneratorObject ItemsObject { get; private set; }
        public IGeneratorObjectPropertyStringList ItemsObjectProperty { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (IndexObject == null || IndexObjectProperty == null)
            {
                IsConnected = true;

                IndexObject = GeneratorObject.GeneratorObjectMap[BaseSelector.IndexObject];
                IndexObjectProperty = (IGeneratorObjectPropertyIndex)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseSelector.IndexObjectProperty];
            }

            if (ItemsResource == null && (ItemsObject == null || ItemsObjectProperty == null))
            {
                IsConnected = true;

                if (BaseSelector.ItemsResource != null)
                {
                    ItemsResource = GeneratorResource.GeneratorResourceMap[BaseSelector.ItemsResource];
                }

                else if (BaseSelector.ItemsObject != null && BaseSelector.ItemsObjectProperty != null)
                {
                    ItemsObject = GeneratorObject.GeneratorObjectMap[BaseSelector.ItemsObject];
                    ItemsObjectProperty = (IGeneratorObjectPropertyStringList)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseSelector.ItemsObjectProperty];
                }
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string Properties = $" Style=\"{{StaticResource {GetStyleResourceKey(design, styleName)}}}\"";
            string IndexValue = GetComponentValue(currentPage, currentObject, null, IndexObject, IndexObjectProperty, null, false);
            string ItemsValue = GetComponentValue(currentPage, currentObject, ItemsResource, ItemsObject, ItemsObjectProperty, null, false);
            string LoadedEvent = currentPage.Dynamic.HasProperties ? $" Loaded=\"{GetLoadedHandlerName(IndexObject, IndexObjectProperty)}\"" : "";
            string ValueChangedEvent = currentPage.Dynamic.HasProperties ? $" SelectionChanged=\"{GetChangedHandlerName(IndexObject, IndexObjectProperty)}\"" : "";

            // SelectedIndex must be first, no clue why.
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<p:ListBox x:Name=\"{ControlName}\"{attachedProperties}{visibilityBinding}{Properties}{elementProperties} ControlSelectedIndex=\"{IndexValue}\"{LoadedEvent}{ValueChangedEvent} ItemsSource=\"{ItemsValue}\"/>");
        }

        public string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            return ComponentSelector.FormatStyleResourceKey(design.XamlName, styleName);
        }

        public IGeneratorObject BoundObject { get { return IndexObject; } }
        public IGeneratorObjectProperty BoundObjectProperty { get { return IndexObjectProperty; } }
        public string HandlerArgumentTypeName { get { return "SelectionChangedEventArgs"; } }
        public bool PostponeChangedNotification { get { return false; } }
    }
}
