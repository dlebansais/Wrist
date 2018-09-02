using System.IO;
using Windows.UI.Xaml;

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
                TextObject = GeneratorObject.GeneratorObjectMap[BasePasswordEdit.TextObject];
                TextObjectProperty = (IGeneratorObjectPropertyString)GeneratorObjectProperty.GeneratorObjectPropertyMap[BasePasswordEdit.TextObjectProperty];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string MaximumLengthProperty = ((TextObjectProperty != null && TextObjectProperty.MaximumLength > 0) ? $" MaxLength=\"{TextObjectProperty.MaximumLength}\"" : "");
            string Properties = $" Style=\"{{StaticResource {GetStyleResourceKey(design, styleName)}}}\"{MaximumLengthProperty}";
            string Value = GetComponentValue(currentPage, currentObject, null, TextObject, TextObjectProperty, null, true);
            string ValueChangedEvent = currentPage.Dynamic.HasProperties ? $" PasswordChanged=\"{PasswordChangedEventName}\"" : "";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<PasswordBox x:Name=\"{ControlName}\"{attachedProperties}{visibilityBinding} Password=\"{Value}\"{ValueChangedEvent}{Properties}{elementProperties}/>");
        }

        public string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            return ComponentPasswordEdit.FormatStyleResourceKey(design.XamlName, styleName);
        }

        public string PasswordChangedEventName
        {
            get { return GetChangedHandlerName(TextObject, TextObjectProperty); }
        }

        public IGeneratorObject BoundObject { get { return TextObject; } }
        public IGeneratorObjectProperty BoundObjectProperty { get { return TextObjectProperty; } }
        public string HandlerArgumentTypeName { get { return "RoutedEventArgs"; } }
        public bool PostponeChangedNotification { get { return false; } }
    }
}
