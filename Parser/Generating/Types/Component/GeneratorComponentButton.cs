using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentButton : GeneratorComponent, IGeneratorComponentButton
    {
        public GeneratorComponentButton(IComponentButton button)
            : base(button)
        {
            BaseButton = button;
            ContentKey = button.ContentKey;
        }

        private IComponentButton BaseButton;

        public IGeneratorResource ContentResource { get; private set; }
        public IGeneratorObject ContentObject { get; private set; }
        public IGeneratorObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public IGeneratorPageNavigation GoTo { get; private set; }
        public IGeneratorObject ClosePopupObject { get; private set; }
        public IGeneratorObjectPropertyBoolean ClosePopupObjectProperty { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            ConnectContent(domain, ref IsConnected);
            ConnectGoTo(domain, ref IsConnected);
            ConnectClosePopup(domain, ref IsConnected);

            return IsConnected;
        }

        public void ConnectContent(IGeneratorDomain domain, ref bool IsConnected)
        {
            if (ContentResource == null && (ContentObject == null || ContentObjectProperty == null))
            {
                IsConnected = true;

                if (BaseButton.ContentResource != null)
                {
                    if (GeneratorResource.GeneratorResourceMap.ContainsKey(BaseButton.ContentResource))
                        ContentResource = GeneratorResource.GeneratorResourceMap[BaseButton.ContentResource];
                }

                else if (BaseButton.ContentObject != null && BaseButton.ContentObjectProperty != null)
                {
                    if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseButton.ContentObject))
                        ContentObject = GeneratorObject.GeneratorObjectMap[BaseButton.ContentObject];
                    if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseButton.ContentObjectProperty))
                        ContentObjectProperty = GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseButton.ContentObjectProperty];
                }
            }
        }

        public void ConnectGoTo(IGeneratorDomain domain, ref bool IsConnected)
        {
            if (GoTo == null)
            {
                GoTo = new GeneratorPageNavigation(BaseButton.GoTo);
                IsConnected = true;
            }
        }

        public void ConnectClosePopup(IGeneratorDomain domain, ref bool IsConnected)
        {
            if ((ClosePopupObject == null || ClosePopupObjectProperty == null) && (BaseButton.ClosePopupObject != null && BaseButton.ClosePopupObjectProperty != null))
            {
                IsConnected = true;

                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseButton.ClosePopupObject))
                    ClosePopupObject = GeneratorObject.GeneratorObjectMap[BaseButton.ClosePopupObject];
                if (GeneratorObjectProperty.GeneratorObjectPropertyMap.ContainsKey(BaseButton.ClosePopupObjectProperty))
                    ClosePopupObjectProperty = (IGeneratorObjectPropertyBoolean)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseButton.ClosePopupObjectProperty];
            }
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string Properties = $" Style=\"{{StaticResource {GetStyleResourceKey(design, styleName)}}}\"";
            string Value = GetComponentValue(currentPage, currentObject, ContentResource, ContentObject, ContentObjectProperty, ContentKey, false);
            string ClickEventHandler = $" Click=\"{ClickEventName(currentPage)}\"";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<Button x:Name=\"{ControlName}\"{attachedProperties}{visibilityBinding}{Properties}{elementProperties}{ClickEventHandler} Content=\"{Value}\"/>");
        }

        public override string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            string StyleProperty = (styleName != null) ? styleName : "";
            return $"{design.XamlName}Button{StyleProperty}";
        }

        public string ClickEventName(IGeneratorPage currentPage)
        {
            if (GoTo.GoToPage == GeneratorPage.CurrentPage)
            {
                IGeneratorPageNavigation Copy = GoTo.CreateCopyForPage(currentPage, this);
                return Copy.EventName;
            }
            else
                return GoTo.EventName;
        }
    }
}
