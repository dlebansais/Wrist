using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentCheckBox : GeneratorComponent, IGeneratorComponentCheckBox
    {
        public static Dictionary<IComponentCheckBox, IGeneratorComponentCheckBox> GeneratorComponentCheckBoxMap { get; } = new Dictionary<IComponentCheckBox, IGeneratorComponentCheckBox>();

        public GeneratorComponentCheckBox(IComponentCheckBox checkbox)
            : base(checkbox)
        {
            BaseCheckBox = checkbox;
            ContentKey = checkbox.ContentKey;

            GeneratorComponentCheckBoxMap.Add(BaseCheckBox, this);
        }

        private IComponentCheckBox BaseCheckBox;

        public IGeneratorResource ContentResource { get; private set; }
        public IGeneratorObject ContentObject { get; private set; }
        public IGeneratorObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public IGeneratorObject CheckedObject { get; private set; }
        public IGeneratorObjectPropertyBoolean CheckedObjectProperty { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            ConnectContent(domain, ref IsConnected);
            ConnectChecked(domain, ref IsConnected);

            return IsConnected;
        }

        public void ConnectContent(IGeneratorDomain domain, ref bool IsConnected)
        {
            if (ContentResource == null && (ContentObject == null || ContentObjectProperty == null))
            {
                IsConnected = true;

                if (BaseCheckBox.ContentResource != null)
                {
                    ContentResource = GeneratorResource.GeneratorResourceMap[BaseCheckBox.ContentResource];
                }

                else if (BaseCheckBox.ContentObject != null && BaseCheckBox.ContentObjectProperty != null)
                {
                    ContentObject = GeneratorObject.GeneratorObjectMap[BaseCheckBox.ContentObject];
                    ContentObjectProperty = GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseCheckBox.ContentObjectProperty];
                }
            }
        }

        public void ConnectChecked(IGeneratorDomain domain, ref bool IsConnected)
        {
            if (CheckedObject == null || CheckedObjectProperty == null)
            {
                IsConnected = true;

                CheckedObject = GeneratorObject.GeneratorObjectMap[BaseCheckBox.CheckedObject];
                CheckedObjectProperty = (IGeneratorObjectPropertyBoolean)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseCheckBox.CheckedObjectProperty];
            }
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string Properties = $" Style=\"{{StaticResource {GetStyleResourceKey(design, styleName)}}}\"";
            string Content = GetComponentValue(currentPage, currentObject, ContentResource, ContentObject, ContentObjectProperty, ContentKey, false);
            string IsCheckedBinding = GetComponentValue(currentPage, currentObject, null, CheckedObject, CheckedObjectProperty, null, true);
            string CheckedEvent = currentPage.Dynamic.HasProperties ? $" Checked=\"{GetChangedHandlerName(CheckedObject, CheckedObjectProperty)}\"" : "";
            string UncheckedEvent = currentPage.Dynamic.HasProperties ? $" Unchecked=\"{GetChangedHandlerName(CheckedObject, CheckedObjectProperty)}\"" : "";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<CheckBox{attachedProperties}{visibilityBinding}{Properties}{elementProperties} IsChecked=\"{IsCheckedBinding}\"{CheckedEvent}{UncheckedEvent} Content=\"{Content}\"/>");
        }

        public string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            return ComponentCheckBox.FormatStyleResourceKey(design.XamlName, styleName);
        }

        public IGeneratorObject BoundObject { get { return CheckedObject; } }
        public IGeneratorObjectProperty BoundObjectProperty { get { return CheckedObjectProperty; } }
        public string HandlerArgumentTypeName { get { return "RoutedEventArgs"; } }
        public bool PostponeChangedNotification { get { return true; } }
    }
}
