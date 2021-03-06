﻿using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentEdit : GeneratorComponent, IGeneratorComponentEdit
    {
        public GeneratorComponentEdit(IComponentEdit edit)
            : base(edit)
        {
            AcceptsReturn = edit.AcceptsReturn;
            TextDecoration = edit.TextDecoration;
            HorizontalScrollBarVisibility = edit.HorizontalScrollBarVisibility;
            VerticalScrollBarVisibility = edit.VerticalScrollBarVisibility;
            BaseEdit = edit;
        }

        private IComponentEdit BaseEdit;

        public IGeneratorObject TextObject { get; private set; }
        public IGeneratorObjectPropertyString TextObjectProperty { get; private set; }
        public bool AcceptsReturn { get; private set; }
        public string TextDecoration { get; private set; }
        public string HorizontalScrollBarVisibility { get; private set; }
        public string VerticalScrollBarVisibility { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (TextObject == null || TextObjectProperty == null)
            {
                TextObject = GeneratorObject.GeneratorObjectMap[BaseEdit.TextObject];
                TextObjectProperty = (IGeneratorObjectPropertyString)GeneratorObjectProperty.GeneratorObjectPropertyMap[BaseEdit.TextObjectProperty];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string MaximumLengthProperty = ((TextObjectProperty != null && TextObjectProperty.MaximumLength > 0) ? $" MaxLength=\"{TextObjectProperty.MaximumLength}\"" : "");
            string AcceptsReturnProperty = (AcceptsReturn ? " AcceptsReturn=\"True\"" : "");
            string AlignmentProperty = (isHorizontalAlignmentStretch ? $" TextAlignment=\"Justify\"" : "");
            string WrappingProperty = ((textWrapping.HasValue && textWrapping.Value == TextWrapping.Wrap) ? " TextWrapping=\"Wrap\"" : " TextWrapping=\"NoWrap\"");
            string DecorationProperty = (TextDecoration != null ? $" TextDecorations=\"{TextDecoration}\"" : "");
            string HorizontalScrollBarProperty = (HorizontalScrollBarVisibility != null ? $" HorizontalScrollBarVisibility=\"{HorizontalScrollBarVisibility}\"" : "");
            string VerticalScrollBarProperty = (VerticalScrollBarVisibility != null ? $" VerticalScrollBarVisibility=\"{VerticalScrollBarVisibility}\"" : "");
            string Properties = $" Style=\"{{StaticResource {GetStyleResourceKey(design, styleName)}}}\"{MaximumLengthProperty}{AcceptsReturnProperty}{AlignmentProperty}{WrappingProperty}{DecorationProperty}{HorizontalScrollBarProperty}{VerticalScrollBarProperty}";
            string Value = GetComponentValue(currentPage, currentObject, null, TextObject, TextObjectProperty, null, true);
            string ValueChangedEvent = currentPage.Dynamic.HasProperties ? $" TextChanged=\"{TextChangedEventName}\"" : "";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<TextBox x:Name=\"{ControlName}\"{attachedProperties}{visibilityBinding} Text=\"{Value}\"{ValueChangedEvent}{Properties}{elementProperties}/>");
        }

        public string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            return ComponentEdit.FormatStyleResourceKey(design.XamlName, styleName);
        }

        public string TextChangedEventName
        {
            get { return GetChangedHandlerName(TextObject, TextObjectProperty); }
        }

        public IGeneratorObject BoundObject { get { return TextObject; } }
        public IGeneratorObjectProperty BoundObjectProperty { get { return TextObjectProperty; } }
        public string HandlerArgumentTypeName { get { return "TextChangedEventArgs"; } }
        public bool PostponeChangedNotification { get { return false; } }
    }
}
