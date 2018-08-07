using System.IO;

namespace Parser
{
    public class GeneratorComponentPasswordEdit : GeneratorComponent, IGeneratorComponentPasswordEdit
    {
        public GeneratorComponentPasswordEdit(IComponentPasswordEdit edit)
            : base(edit)
        {
            BasePasswordEdit = edit;
        }

        private IComponentPasswordEdit BasePasswordEdit;

        public IGeneratorObject TextObject { get; private set; }
        public IGeneratorObjectPropertyString TextObjectProperty { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (TextObject == null || TextObjectProperty == null)
            {
                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BasePasswordEdit.TextObject))
                    TextObject = GeneratorObject.GeneratorObjectMap[BasePasswordEdit.TextObject];
                if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BasePasswordEdit.TextObjectProperty))
                    TextObjectProperty = (IGeneratorObjectPropertyString)GeneratorObjectProperty.GeneratorObjectPropertyMap[BasePasswordEdit.TextObjectProperty];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (style != null) ? style : "";
            string MaximumLengthProperty = ((TextObjectProperty != null && TextObjectProperty.MaximumLength > 0) ? $" MaxLength=\"{TextObjectProperty.MaximumLength}\"" : "");
            string Properties = $" Style=\"{{StaticResource {design.XamlName}PasswordBox{StyleProperty}}}\"{MaximumLengthProperty}";
            string Value = GetComponentValue(currentPage, currentObject, null, TextObject, TextObjectProperty, null, true);

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<PasswordBox{attachedProperties}{visibilityBinding} Password=\"{Value}\"{Properties}{elementProperties}/>");
        }
    }
}
