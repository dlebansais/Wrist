namespace Parser
{
    public class ComponentImage : Component, IComponentImage
    {
        public ComponentImage(IDeclarationSource source, string xamlName, IComponentProperty sourceProperty, bool isResourceWidth, double width, bool isResourceHeight, double height)
            : base(source, xamlName)
        {
            SourceProperty = sourceProperty;
            Width = width;
            IsResourceWidth = isResourceWidth;
            Height = height;
            IsResourceHeight = isResourceHeight;
        }

        public IComponentProperty SourceProperty { get; private set; }
        public IResource SourceResource { get; private set; }
        public bool IsResourceWidth { get; private set; }
        public double Width { get; private set; }
        public bool IsResourceHeight { get; private set; }
        public double Height { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            IResource Resource = SourceResource;
            bool IsConnected = SourceProperty.ConnectToResourceOnly(domain, ref Resource);
            SourceResource = Resource;

            if (!IsConnected && SourceResource != null)
            {
                if (IsResourceWidth && Width != SourceResource.Width)
                {
                    IsConnected = true;
                    Width = SourceResource.Width;
                }

                if (IsResourceHeight && Height != SourceResource.Height)
                {
                    IsConnected = true;
                    Height = SourceResource.Height;
                }
            }

            return IsConnected;
        }

        public override string ToString()
        {
            string SizeString = (!double.IsNaN(Width) && double.IsNaN(Height)) ? $" ({Width}x{Height})" : "";
            return $"{GetType().Name} '{Source.Name}'{SizeString}";
        }
    }
}
