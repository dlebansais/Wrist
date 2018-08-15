﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorControl : GeneratorLayoutElement, IGeneratorControl
    {
        public GeneratorControl(Control control)
            : base(control)
        {
            Name = control.Name;
            TextWrapping = control.TextWrapping;
        }

        public string Name { get; private set; }
        public IGeneratorComponent Component { get; private set; }
        public Windows.UI.Xaml.TextWrapping? TextWrapping { get; private set; }

        public override bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            bool IsConnected = false;

            if (Component == null)
            {
                foreach (IGeneratorComponent Item in components)
                    if (Item.Source.Name == Name)
                    {
                        Component = Item;
                        break;
                    }

                if (Component == null)
                    throw new InvalidOperationException();

                IsConnected = true;
            }

            return IsConnected;
        }

        public override void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            bool IsHorizontalAlignmentStretch = (HorizontalAlignment == Windows.UI.Xaml.HorizontalAlignment.Stretch.ToString());
            string AttachedPropertiesString = AttachedProperties(this);
            string ElementPropertiesString = ElementProperties(currentPage, currentObject);

            Component.Generate(design, Style, AttachedPropertiesString, ElementPropertiesString, TextWrapping, IsHorizontalAlignmentStretch, indentation, currentPage, currentObject, colorTheme, xamlWriter, visibilityBinding);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
