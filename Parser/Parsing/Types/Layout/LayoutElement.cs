using System.Collections.Generic;

namespace Parser
{
    public abstract class LayoutElement : ILayoutElement
    {
        public LayoutElement()
        {
            Source = ParsingSourceStream.GetCurrentSource();
        }

        public IParsingSource Source { get; private set; }
        public string Style { get; set; }
        public string Background { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Margin { get; set; }
        public string HorizontalAlignment { get; set; }
        public string VerticalAlignment { get; set; }
        public string ElementEnable { get; set; }
        public IComponent ControllerElement { get; private set; }

        public virtual void ConnectComponents(IDomain domain, IReadOnlyCollection<IComponent> components)
        {
            double WidthValue;
            if (Width != null && !double.TryParse(Width, out WidthValue))
                throw new ParsingException(193, Source, "Invalid width.");

            double HeightValue;
            if (Height != null && !double.TryParse(Height, out HeightValue))
                throw new ParsingException(194, Source, "Invalid height.");

            double SingleMargin;
            if (Margin != null && !double.TryParse(Margin, out SingleMargin))
            {
                string[] ThicknessMargin = Margin.Split(',');
                int i;

                for (i = 0; i < 4 && i < ThicknessMargin.Length; i++)
                    if (!double.TryParse(ThicknessMargin[i], out SingleMargin))
                        break;

                if (i < 4)
                    throw new ParsingException(195, Source, "Invalid margin.");
            }

            if (HorizontalAlignment != null &&
                HorizontalAlignment != Windows.UI.Xaml.HorizontalAlignment.Left.ToString() &&
                HorizontalAlignment != Windows.UI.Xaml.HorizontalAlignment.Center.ToString() &&
                HorizontalAlignment != Windows.UI.Xaml.HorizontalAlignment.Right.ToString() &&
                HorizontalAlignment != Windows.UI.Xaml.HorizontalAlignment.Stretch.ToString())
                throw new ParsingException(196, Source, "Invalid horizontal alignment.");

            if (VerticalAlignment != null &&
                VerticalAlignment != Windows.UI.Xaml.VerticalAlignment.Top.ToString() &&
                VerticalAlignment != Windows.UI.Xaml.VerticalAlignment.Center.ToString() &&
                VerticalAlignment != Windows.UI.Xaml.VerticalAlignment.Bottom.ToString() &&
                VerticalAlignment != Windows.UI.Xaml.VerticalAlignment.Stretch.ToString())
                throw new ParsingException(197, Source, "Invalid vertical alignment.");

            if (ElementEnable != null && ControllerElement == null)
            {
                foreach (IComponent Component in components)
                    if (Component.Source.Name == ElementEnable)
                    {
                        ControllerElement = Component;
                        break;
                    }

                if (ControllerElement == null)
                    throw new ParsingException(198, Source, $"Element '{ElementEnable}' not found.");
                else if (ControllerElement is IComponentRadioButton AsRadioButton)
                    AsRadioButton.SetController();
                else if (ControllerElement is IComponentCheckBox AsCheckBox)
                    AsCheckBox.SetController();
                else
                    throw new ParsingException(199, Source, $"Element '{ElementEnable}' is neither a CheckBox or RadioButton.");
            }
        }
    }
}
