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
            IsController = checkbox.IsController;

            GeneratorComponentCheckBoxMap.Add(BaseCheckBox, this);
        }

        private IComponentCheckBox BaseCheckBox;

        public IGeneratorResource ContentResource { get; private set; }
        public IGeneratorObject ContentObject { get; private set; }
        public IGeneratorObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public IGeneratorObject CheckedObject { get; private set; }
        public IGeneratorObjectPropertyBoolean CheckedObjectProperty { get; private set; }
        public bool IsController { get; private set; }

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
                    if (GeneratorResource.GeneratorResourceMap.ContainsKey(BaseCheckBox.ContentResource))
                        ContentResource = GeneratorResource.GeneratorResourceMap[BaseCheckBox.ContentResource];
                }

                else if (BaseCheckBox.ContentObject != null && BaseCheckBox.ContentObjectProperty != null)
                {
                    if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseCheckBox.ContentObject))
                        ContentObject = GeneratorObject.GeneratorObjectMap[BaseCheckBox.ContentObject];
                    if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseCheckBox.ContentObjectProperty))
                        ContentObjectProperty = GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseCheckBox.ContentObjectProperty];
                }
            }
        }

        public void ConnectChecked(IGeneratorDomain domain, ref bool IsConnected)
        {
            if (CheckedObject == null || CheckedObjectProperty == null)
            {
                IsConnected = true;

                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseCheckBox.CheckedObject))
                    CheckedObject = GeneratorObject.GeneratorObjectMap[BaseCheckBox.CheckedObject];
                if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseCheckBox.CheckedObjectProperty))
                    CheckedObjectProperty = (IGeneratorObjectPropertyBoolean)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseCheckBox.CheckedObjectProperty];
            }
        }

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (style != null) ? style : "";
            string NameProperty = IsController ? $" x:Name=\"{XamlName}\"" : "";
            string Properties = $" Style=\"{{StaticResource {design.XamlName}CheckBox{StyleProperty}}}\"";
            string Content = GetComponentValue(currentPage, currentObject, ContentResource, ContentObject, ContentObjectProperty, ContentKey, false);
            string IsCheckedBinding = GetComponentValue(currentPage, currentObject, null, CheckedObject, CheckedObjectProperty, null, true);

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<CheckBox{NameProperty}{attachedProperties}{visibilityBinding}{Properties}{elementProperties} IsChecked=\"{IsCheckedBinding}\" Content=\"{Content}\"/>");
        }
    }
}
