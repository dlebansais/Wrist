using System.IO;

namespace Parser
{
    public class GeneratorComponentCheckBox : GeneratorComponent, IGeneratorComponentCheckBox
    {
        public GeneratorComponentCheckBox(IComponentCheckBox checkbox)
            : base(checkbox)
        {
            BaseCheckBox = checkbox;
            ContentKey = checkbox.ContentKey;
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

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (style != null) ? style : "";
            string Properties = $" Style=\"{{StaticResource {design.XamlName}CheckBox{StyleProperty}}}\"";
            string Value = GetComponentValue(currentPage, currentObject, ContentResource, ContentObject, ContentObjectProperty, ContentKey, true);
            string CheckedProperty = "";

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<CheckBox{attachedProperties}{visibilityBinding}{Properties}{elementProperties}{CheckedProperty} Content=\"{Value}\"/>");
        }
    }
}
