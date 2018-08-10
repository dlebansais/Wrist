namespace Parser
{
    public class ComponentImage : Component, IComponentImage
    {
        public ComponentImage(IDeclarationSource source, string xamlName, IComponentProperty sourceProperty, string imageWidth, string imageHeight)
            : base(source, xamlName)
        {
            SourceProperty = sourceProperty;
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
        }

        public IComponentProperty SourceProperty { get; private set; }
        public string ImageWidth { get; private set; }
        public string ImageHeight { get; private set; }
        public IResource SourceResource { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            IResource Resource = SourceResource;
            bool IsConnected = SourceProperty.ConnectToResource(domain, ref Resource);
            SourceResource = Resource;

            double WidthValue;
            if (!double.TryParse(ImageWidth, out WidthValue))
                throw new ParsingException(Source.Source, $"{ImageWidth} not parsed as a width");

            Width = WidthValue;

            double HeightValue;
            if (!double.TryParse(ImageHeight, out HeightValue))
                throw new ParsingException(Source.Source, $"{ImageHeight} not parsed as a height");

            Height = HeightValue;

            return IsConnected;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Source.Name}' ({Width}x{Height})";
        }
    }
}
