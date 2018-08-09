using System.Collections.Generic;

namespace Parser
{
    public class ComponentRadioButton : Component, IComponentRadioButton
    {
        public ComponentRadioButton(IDeclarationSource source, string xamlName, IComponentProperty contentProperty, string groupName)
            : base(source, xamlName)
        {
            ContentProperty = contentProperty;
            GroupName = groupName;
        }

        public IComponentProperty ContentProperty { get; private set; }
        public IResource ContentResource { get; private set; }
        public IObject ContentObject { get; private set; }
        public IObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public string GroupName { get; private set; }
        public ICollection<IComponentRadioButton> Group { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectContent(domain, currentObject, ref IsConnected);
            ConnectGroup(domain, rootArea, ref IsConnected);

            return IsConnected;
        }

        private void ConnectContent(IDomain domain, IObject currentObject, ref bool IsConnected)
        {
            IResource Resource = ContentResource;
            IObject Object = ContentObject;
            IObjectProperty ObjectProperty = ContentObjectProperty;
            IDeclarationSource ObjectPropertyKey = ContentKey;
            IsConnected |= ContentProperty.ConnectToResourceOrObject(domain, currentObject, ref Resource, ref Object, ref ObjectProperty, ref ObjectPropertyKey);
            ContentResource = Resource;
            ContentObject = Object;
            ContentObjectProperty = ObjectProperty;
            ContentKey = ObjectPropertyKey;
        }

        private void ConnectGroup(IDomain domain, IArea rootArea, ref bool IsConnected)
        {
            if (Group == null && !IsConnected) // Ensure this is checked during the second pass only, when all areas are connected.
            {
                IsConnected = true;

                if (rootArea != null)
                    throw new ParsingException(Source.Source, $"Radio button ${Source.Name} no referenced in a page");

                Group = new List<IComponentRadioButton>();
                rootArea.FindOtherRadioButtons(GroupName, Group);

                if (Group.Count < 2)
                    throw new ParsingException(Source.Source, $"Group name {GroupName} is only referencing one radio button");
            }
        }
    }
}
