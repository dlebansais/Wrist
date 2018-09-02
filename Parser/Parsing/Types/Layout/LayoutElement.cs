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
        public string Width { get; set; }
        public string Height { get; set; }
        public string Margin { get; set; }
        public string HorizontalAlignment { get; set; }
        public string VerticalAlignment { get; set; }
        public string DynamicEnable { get; set; }
        public IDynamicProperty DynamicController { get; private set; }

        public abstract ILayoutElement GetClone();

        protected virtual void InitializeClone(LayoutElement clone)
        {
            clone.Source = Source;
            clone.Style = Style;
            clone.Width = Width;
            clone.Height = Height;
            clone.Margin = Margin;
            clone.HorizontalAlignment = HorizontalAlignment;
            clone.DynamicEnable = DynamicEnable;

            DockPanel.CloneDock(this, clone);
            Grid.CloneColumn(this, clone);
            Grid.CloneColumnSpan(this, clone);
            Grid.CloneRow(this, clone);
            Grid.CloneRowSpan(this, clone);
        }

        public virtual void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            double WidthValue;
            if (Width != null && !ParserDomain.TryParseDouble(Width, out WidthValue))
                throw new ParsingException(193, Source, "Invalid width.");

            double HeightValue;
            if (Height != null && !ParserDomain.TryParseDouble(Height, out HeightValue))
                throw new ParsingException(194, Source, "Invalid height.");

            double SingleMargin;
            if (Margin != null && !ParserDomain.TryParseDouble(Margin, out SingleMargin))
            {
                string[] ThicknessMargin = Margin.Split(',');
                int i;

                for (i = 0; i < 4 && i < ThicknessMargin.Length; i++)
                    if (!ParserDomain.TryParseDouble(ThicknessMargin[i], out SingleMargin))
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

            if (DynamicEnable != null && DynamicController == null)
            {
                foreach (IDynamicProperty Item in currentDynamic.Properties)
                    if (DynamicEnable == Item.Source.Name)
                    {
                        DynamicController = Item;
                        break;
                    }

                if (DynamicController == null)
                    throw new ParsingException(198, Source, $"Dynamic property '{DynamicEnable}' not found in '{currentDynamic.Name}'.");
                else if (DynamicController.Result != DynamicOperationResults.Boolean)
                    throw new ParsingException(199, Source, $"Dynamic property '{DynamicEnable}' is not boolean.");

                DynamicController.SetIsUsed();
            }
        }

        public abstract void ReportResourceKeys(IDesign design, List<string> KeyList);
    }
}
