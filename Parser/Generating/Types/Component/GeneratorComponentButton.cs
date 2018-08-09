﻿using System.IO;
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

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            ConnectContent(domain, ref IsConnected);
            ConnectGoTo(domain, ref IsConnected);

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

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (style != null) ? style : "";
            string Properties = $" Style=\"{{StaticResource {design.XamlName}Button{StyleProperty}}}\"";
            string Value = GetComponentValue(currentPage, currentObject, ContentResource, ContentObject, ContentObjectProperty, ContentKey, false);

            string ClickEventHandler;
            if (GoTo.GoToPage == GeneratorPage.CurrentPage)
            {
                IGeneratorPageNavigation Copy = GoTo.CreateCopyForPage(currentPage, this);
                ClickEventHandler = $" Click=\"{Copy.EventName}\"";
            }
            else
                ClickEventHandler = $" Click=\"{GoTo.EventName}\"";

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<Button{attachedProperties}{visibilityBinding}{Properties}{elementProperties}{ClickEventHandler} Content=\"{Value}\"/>");
        }
    }
}
