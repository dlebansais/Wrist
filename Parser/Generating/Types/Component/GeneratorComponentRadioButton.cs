﻿using System.Collections.Generic;
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
            GroupName = radioButton.GroupName;
            ContentKey = radioButton.ContentKey;
            BaseRadioButton = radioButton;

            GeneratorComponentRadioButtonMap.Add(BaseRadioButton, this);
        }

        private IComponentRadioButton BaseRadioButton;

        public IGeneratorResource ContentResource { get; private set; }
        public IGeneratorObject ContentObject { get; private set; }
        public IGeneratorObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public string GroupName { get; private set; }
        public ICollection<IGeneratorComponentRadioButton> Group { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            ConnectContent(domain, ref IsConnected);
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

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string StyleProperty = (style != null) ? style : "";
            string Properties = $" Style=\"{{StaticResource {design.XamlName}RadioButton{StyleProperty}}}\" GroupName=\"{GroupName}\"";
            string Value = GetComponentValue(currentPage, currentObject, ContentResource, ContentObject, ContentObjectProperty, ContentKey, false);

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<RadioButton{attachedProperties}{visibilityBinding}{Properties}{elementProperties} Content=\"{Value}\"/>");
        }
    }
}
