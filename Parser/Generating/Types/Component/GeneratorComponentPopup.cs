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
        public IGeneratorArea Area { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public override bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            if (Area == null)
            {
                IsConnected = true;
                if (GeneratorArea.GeneratorAreaMap.ContainsKey(BasePopup.Area))
                    Area = GeneratorArea.GeneratorAreaMap[BasePopup.Area];
            }

            if (SourceResource == null)
            {
                IsConnected = true;
                if (GeneratorResource.GeneratorResourceMap.ContainsKey(BasePopup.SourceResource))
                    SourceResource = GeneratorResource.GeneratorResourceMap[BasePopup.SourceResource];
            }

            return IsConnected;
        }

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string BindingName = $"{XamlName}_Toggle";
            string OpeningBinding = $" IsOpen=\"{{Binding IsChecked, ElementName={BindingName}}}\"";
            string PanelProperties = " VerticalAlignment=\"Bottom\" HorizontalAlignment=\"Right\"";
            string ButtonProperties = $" HorizontalAlignment=\"Right\" Style=\"{{StaticResource {design.XamlName}ToggleButton}}\"";
            string PopupProperties = " HorizontalOffset=\"0\" VerticalOffset=\"0\"";
            string AreaProperties = $" Template=\"{{StaticResource {Area.XamlName}}}\"";
            string StyleProperty = (style != null) ? style : "";
            string ImageProperties = $" Style=\"{{StaticResource {design.XamlName}Image{StyleProperty}}}\"";
            string ImageSource = $" Source=\"{GetComponentValue(currentPage, currentObject, SourceResource, null, null, null, false)}\"";
            string WidthProperty = double.IsNaN(Width) ? "" : $" Width=\"{Width}\"";
            string HeightProperty = double.IsNaN(Height) ? "" : $" Height=\"{Height}\"";

            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}<StackPanel{attachedProperties}{visibilityBinding}{PanelProperties}{elementProperties}>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    <Grid>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        <ContentControl{AreaProperties} Height=\"0\" Opacity=\"0\"/>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        <ToggleButton x:Name=\"{BindingName}\"{ButtonProperties}>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}            <Image{ImageProperties}{ImageSource}{WidthProperty}{HeightProperty}/>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        </ToggleButton>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    </Grid>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    <p:Popup{OpeningBinding}{PopupProperties}>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        <Border>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}            <StackPanel>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}                <ContentControl{AreaProperties}/>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}            </StackPanel>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}        </Border>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}    </p:Popup>");
            colorTheme.WriteXamlLine(xamlWriter, $"{Indentation}</StackPanel>");
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
