using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentRadioButton : GeneratorComponent, IGeneratorComponentRadioButton
    {
        public static Dictionary<IComponentRadioButton, IGeneratorComponentRadioButton> GeneratorComponentRadioButtonMap { get; } = new Dictionary<IComponentRadioButton, IGeneratorComponentRadioButton>();

        public GeneratorComponentRadioButton(IComponentRadioButton radioButton)
            : base(radioButton)
        {
            ContentKey = radioButton.ContentKey;
            GroupName = radioButton.GroupName;
            GroupIndex = radioButton.GroupIndex;
            BaseRadioButton = radioButton;

            GeneratorComponentRadioButtonMap.Add(BaseRadioButton, this);
        }

        private IComponentRadioButton BaseRadioButton;

        public IGeneratorResource ContentResource { get; private set; }
        public IGeneratorObject ContentObject { get; private set; }
        public IGeneratorObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public IGeneratorObject IndexObject { get; private set; }
        public IGeneratorObjectPropertyIndex IndexObjectProperty { get; private set; }
        public string GroupName { get; private set; }
        public int GroupIndex { get; private set; }
        public ICollection<IGeneratorComponentRadioButton> Group { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            ConnectContent(domain, ref IsConnected);
            ConnectIndex(domain, ref IsConnected);
            ConnectGroup(domain, ref IsConnected);

            return IsConnected;
        }

        public void ConnectContent(IGeneratorDomain domain, ref bool IsConnected)
        {
            if (ContentResource == null && (ContentObject == null || ContentObjectProperty == null))
            {
                IsConnected = true;

                if (BaseRadioButton.ContentResource != null)
                {
                    if (GeneratorResource.GeneratorResourceMap.ContainsKey(BaseRadioButton.ContentResource))
                        ContentResource = GeneratorResource.GeneratorResourceMap[BaseRadioButton.ContentResource];
                }

                else if (BaseRadioButton.ContentObject != null && BaseRadioButton.ContentObjectProperty != null)
                {
                    if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseRadioButton.ContentObject))
                        ContentObject = GeneratorObject.GeneratorObjectMap[BaseRadioButton.ContentObject];
                    if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseRadioButton.ContentObjectProperty))
                        ContentObjectProperty = GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseRadioButton.ContentObjectProperty];
                }
            }
        }

        public void ConnectIndex(IGeneratorDomain domain, ref bool IsConnected)
        {
            if (IndexObject == null || IndexObjectProperty == null)
            {
                IsConnected = true;

                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseRadioButton.IndexObject))
                    IndexObject = GeneratorObject.GeneratorObjectMap[BaseRadioButton.IndexObject];
                if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseRadioButton.IndexObjectProperty))
                    IndexObjectProperty = (IGeneratorObjectPropertyIndex)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseRadioButton.IndexObjectProperty];
            }
        }

        public void ConnectGroup(IGeneratorDomain domain, ref bool IsConnected)
        {
            if (Group == null)
            {
                Group = new List<IGeneratorComponentRadioButton>();
                foreach (IComponentRadioButton Item in BaseRadioButton.Group)
                    Group.Add(GeneratorComponentRadioButtonMap[Item]);

                IsConnected = true;
            }
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string Properties = $" Style=\"{{StaticResource {GetStyleResourceKey(design, styleName)}}}\" GroupName=\"{GroupName}\"";
            string ContentValue = GetComponentValue(currentPage, currentObject, ContentResource, ContentObject, ContentObjectProperty, ContentKey, false);
            string IndexValue = GetObjectBinding(currentObject, IndexObject, IndexObjectProperty);
            string IsCheckedBinding = $"{{Binding {IndexValue}, Mode=TwoWay, Converter={{StaticResource convIndexToChecked}}, ConverterParameter={GroupIndex}}}";
            string CheckedEvent = currentPage.Dynamic.HasProperties ? $" Checked=\"{GetChangedHandlerName(IndexObject, IndexObjectProperty)}\"" : "";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<RadioButton{attachedProperties}{visibilityBinding}{Properties}{elementProperties} IsChecked=\"{IsCheckedBinding}\"{CheckedEvent} Content=\"{ContentValue}\"/>");
        }

        public override string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            return ComponentRadioButton.FormatStyleResourceKey(design.XamlName, styleName);
        }

        public IGeneratorObject BoundObject { get { return IndexObject; } }
        public IGeneratorObjectProperty BoundObjectProperty { get { return IndexObjectProperty; } }
        public string HandlerArgumentTypeName { get { return "RoutedEventArgs"; } }
        public bool PostponeChangedNotification { get { return true; } }
    }
}
