using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentPopup : GeneratorComponent, IGeneratorComponentPopup
    {
        public GeneratorComponentPopup(IComponentPopup popup)
            : base(popup)
        {
            Width = popup.Width;
            Height = popup.Height;
            BasePopup = popup;
        }

        protected IComponentPopup BasePopup;

        public IGeneratorResource SourceResource { get; private set; }
        public IGeneratorResource SourcePressedResource { get; private set; }
        public IGeneratorArea Area { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (SourceResource == null)
            {
                IsConnected = true;
                if (GeneratorResource.GeneratorResourceMap.ContainsKey(BasePopup.SourceResource))
                    SourceResource = GeneratorResource.GeneratorResourceMap[BasePopup.SourceResource];
            }

            if (SourcePressedResource == null && BasePopup.SourcePressedResource != null)
            {
                IsConnected = true;
                if (GeneratorResource.GeneratorResourceMap.ContainsKey(BasePopup.SourcePressedResource))
                    SourcePressedResource = GeneratorResource.GeneratorResourceMap[BasePopup.SourcePressedResource];
            }

            if (Area == null)
            {
                IsConnected = true;
                if (GeneratorArea.GeneratorAreaMap.ContainsKey(BasePopup.Area))
                    Area = GeneratorArea.GeneratorAreaMap[BasePopup.Area];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string OpeningBinding = $" IsOpen=\"{{Binding IsChecked, ElementName={ControlName}}}\"";
            string PanelProperties = " HorizontalAlignment=\"Right\"";
            string ButtonProperties = $" HorizontalAlignment=\"Right\" Style=\"{{StaticResource {design.XamlName}ToggleButton}}\"";
            string PopupProperties = " HorizontalOffset=\"0\" VerticalOffset=\"0\"";
            string AreaProperties = $" Template=\"{{StaticResource {Area.XamlName}}}\"";
            string ImageProperties = $" Style=\"{{StaticResource {GetStyleResourceKey(design, styleName)}}}\"";
            string ImageSource = $" Source=\"{GetComponentValue(currentPage, currentObject, SourceResource, null, null, null, false)}\"";
            string WidthProperty = double.IsNaN(Width) ? "" : $" Width=\"{Width}\"";
            string HeightProperty = double.IsNaN(Height) ? "" : $" Height=\"{Height}\"";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<StackPanel{attachedProperties}{visibilityBinding}{PanelProperties}{elementProperties}>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    <Grid>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        <ContentControl{AreaProperties} Height=\"0\" Opacity=\"0\"/>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        <ToggleButton x:Name=\"{ControlName}\"{ButtonProperties} Loaded=\"OnToggleLoaded\">");

            if (SourcePressedResource != null)
            {
                string ImageSourcePressed = $" Source=\"{GetComponentValue(currentPage, currentObject, SourcePressedResource, null, null, null, false)}\"";
                string ImageVisibilityBinding = $" Visibility=\"{{Binding IsChecked, ElementName={ControlName}, Converter={{StaticResource convIndexToVisibility}}, ConverterParameter=1}}\"";
                string ImagePressedVisibilityBinding = $" Visibility=\"{{Binding IsChecked, ElementName={ControlName}, Converter={{StaticResource convIndexToVisibility}}, ConverterParameter=0}}\"";
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}            <Grid>");
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}                <Image{ImageVisibilityBinding}{ImageProperties}{ImageSourcePressed}{WidthProperty}{HeightProperty}/>");
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}                <Image{ImagePressedVisibilityBinding}{ImageProperties}{ImageSource}{WidthProperty}{HeightProperty}/>");
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}            </Grid>");
            }
            else
                colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}            <Image{ImageProperties}{ImageSource}{WidthProperty}{HeightProperty}/>");

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        </ToggleButton>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    </Grid>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    <p:Popup{OpeningBinding}{PopupProperties}>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        <ContentControl{AreaProperties}/>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    </p:Popup>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}</StackPanel>");
        }

        public override string GetStyleResourceKey(IGeneratorDesign design, string styleName)
        {
            string StyleProperty = (styleName != null) ? styleName : "";
            return $"{design.XamlName}Image{StyleProperty}";
        }

        public override bool IsReferencing(IGeneratorArea other)
        {
            if (Area == other)
                return true;

            else if (other.IsReferencedBy(Area))
                return true;

            else
                return false;
        }
    }
}
