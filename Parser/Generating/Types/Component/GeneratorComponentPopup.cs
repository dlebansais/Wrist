using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public class GeneratorComponentPopup : GeneratorComponent, IGeneratorComponentPopup
    {
        public GeneratorComponentPopup(IComponentPopup popup)
            : base(popup)
        {
            BasePopup = popup;
        }

        protected IComponentPopup BasePopup;

        public IGeneratorResource SourceResource { get; private set; }
        public IGeneratorArea Area { get; private set; }

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

        public override void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding)
        {
            string Indentation = GeneratorLayout.IndentationString(indentation);
            string BindingName = $"{XamlName}_Toggle";
            string OpeningBinding = $" IsOpen=\"{{Binding IsChecked, ElementName={BindingName}}}\"";
            string PanelProperties = " VerticalAlignment=\"Bottom\"";
            string ButtonProperties = " Padding=\"0\" BorderThickness=\"0\" HorizontalAlignment=\"Right\"";
            string PopupProperties = " HorizontalOffset=\"0\" VerticalOffset=\"0\"";
            string AreaProperties = $" Template=\"{{StaticResource {Area.XamlName}}}\"";
            string StyleProperty = (style != null) ? style : "";
            string ImageProperties = $" Style=\"{{StaticResource {design.XamlName}Image{StyleProperty}}}\" Width=\"16\" Height=\"16\"";
            string ImageSource = $" Source=\"{GetComponentValue(currentPage, currentObject, SourceResource, null, null, null, false)}\"";

            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}<StackPanel{attachedProperties}{visibilityBinding}{PanelProperties}{elementProperties}>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}    <Grid>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}        <ContentControl{AreaProperties} Height=\"0\"/>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}        <ToggleButton x:Name=\"{BindingName}\"{ButtonProperties}>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}            <Image{ImageProperties}{ImageSource}/>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}        </ToggleButton>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}    </Grid>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}    <p:Popup{OpeningBinding}{PopupProperties}>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}        <Border>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}            <StackPanel>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}                <ContentControl{AreaProperties}/>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}            </StackPanel>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}        </Border>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}    </p:Popup>");
            colorScheme.WriteXamlLine(xamlWriter, $"{Indentation}</StackPanel>");
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
