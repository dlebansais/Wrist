﻿using System.Collections.Generic;

namespace Parser
{
    public class ComponentPopup : Component, IComponentPopup
    {
        public ComponentPopup(IDeclarationSource source, string xamlName, IComponentProperty sourceProperty, IComponentProperty sourcePressedProperty, IDeclarationSource areaSource, double width, double height)
            : base(source, xamlName)
        {
            SourceProperty = sourceProperty;
            SourcePressedProperty = sourcePressedProperty;
            AreaSource = areaSource;
            Width = width;
            Height = height;
        }

        public IComponentProperty SourceProperty { get; private set; }
        public IResource SourceResource { get; private set; }
        public IComponentProperty SourcePressedProperty { get; private set; }
        public IResource SourcePressedResource { get; private set; }
        public IDeclarationSource AreaSource { get; private set; }
        public IArea Area { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectSource(domain, ref IsConnected);
            ConnectSourcePressed(domain, ref IsConnected);
            ConnectArea(domain, ref IsConnected);

            return IsConnected;
        }

        private void ConnectSource(IDomain domain, ref bool IsConnected)
        {
            IResource Resource = SourceResource;
            IsConnected |= SourceProperty.ConnectToResourceOnly(domain, ref Resource);
            SourceResource = Resource;
        }

        private void ConnectSourcePressed(IDomain domain, ref bool IsConnected)
        {
            if (SourcePressedProperty != null)
            {
                IResource Resource = SourcePressedResource;
                IsConnected |= SourcePressedProperty.ConnectToResourceOnly(domain, ref Resource);
                SourcePressedResource = Resource;
            }
            else
                SourcePressedResource = null;
        }

        private void ConnectArea(IDomain domain, ref bool IsConnected)
        {
            if (Area == null)
            {
                if (AreaSource.Name == Parser.Area.EmptyArea.Name)
                    Area = Parser.Area.EmptyArea;
                else
                {
                    foreach (IArea Item in domain.Areas)
                        if (Item.Name == AreaSource.Name)
                        {
                            Area = Item;
                            break;
                        }

                    if (Area == null)
                        throw new ParsingException(117, Source.Source, $"Unknown area '{AreaSource.Name}'.");

                    Area.SetIsUsed();
                }

                IsConnected = true;
            }
        }

        public override bool IsReferencing(IArea other)
        {
            if (Area == other)
                return true;

            else if (Area == Parser.Area.EmptyArea)
                return false;

            else if (other.IsReferencedBy(Area))
                return true;

            else
                return false;
        }

        public override void ReportResourceKeys(IDesign design, List<string> KeyList, string styleName)
        {
            string Key = FormatToggleButtonStyleResourceKey(design.XamlName, styleName);
            if (!KeyList.Contains(Key))
                KeyList.Add(Key);

            Key = ComponentImage.FormatStyleResourceKey(design.XamlName, styleName);
            if (!KeyList.Contains(Key))
                KeyList.Add(Key);
        }

        public static string FormatToggleButtonStyleResourceKey(string xamlDesignName, string styleName)
        {
            string StyleProperty = (styleName != null) ? styleName : "";
            return $"{xamlDesignName}ToggleButton{StyleProperty}";
        }

        public static string FormatImageStyleResourceKey(string xamlDesignName, string styleName)
        {
            string StyleProperty = (styleName != null) ? styleName : "";
            return $"{xamlDesignName}Image{StyleProperty}";
        }

        public override string ToString()
        {
            string SizeString = (!double.IsNaN(Width) && double.IsNaN(Height)) ? $" ({Width}x{Height})" : "";
            return $"{GetType().Name} '{Source.Name}'{SizeString}";
        }
    }
}
