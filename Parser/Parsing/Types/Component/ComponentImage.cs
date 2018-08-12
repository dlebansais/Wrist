namespace Parser
{
    public class ComponentImage : Component, IComponentImage
    {
        public ComponentImage(IDeclarationSource source, string xamlName, IComponentProperty sourceProperty, double width, double height)
            : base(source, xamlName)
        {
            SourceProperty = sourceProperty;
            Width = width;
            Height = height;
        }

        public IComponentProperty SourceProperty { get; private set; }
        public IResource SourceResource { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            IResource Resource = SourceResource;
            bool IsConnected = SourceProperty.ConnectToResource(domain, ref Resource);
            SourceResource = Resource;

            return IsConnected;
        }

        public override string ToString()
        {
            string SizeString = (!double.IsNaN(Width) && double.IsNaN(Height)) ? $" ({Width}x{Height})" : "";
            return $"{GetType().Name} '{Source.Name}'{SizeString}";
        }
    }
}
