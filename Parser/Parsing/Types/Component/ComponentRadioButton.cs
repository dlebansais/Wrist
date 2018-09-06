using System.Collections.Generic;

namespace Parser
{
    public class ComponentRadioButton : Component, IComponentRadioButton
    {
        public ComponentRadioButton(IDeclarationSource source, string xamlName, IComponentProperty contentProperty, IComponentProperty indexProperty, string groupName, int groupIndex)
            : base(source, xamlName)
        {
            ContentProperty = contentProperty;
            IndexProperty = indexProperty;
            GroupName = groupName;
            GroupIndex = groupIndex;
        }

        public IComponentProperty ContentProperty { get; private set; }
        public IResource ContentResource { get; private set; }
        public IObject ContentObject { get; private set; }
        public IObjectProperty ContentObjectProperty { get; private set; }
        public IDeclarationSource ContentKey { get; private set; }
        public IComponentProperty IndexProperty { get; private set; }
        public IObject IndexObject { get; private set; }
        public IObjectPropertyIndex IndexObjectProperty { get; private set; }
        public string GroupName { get; private set; }
        public int GroupIndex { get; private set; }
        public ICollection<IComponentRadioButton> Group { get; private set; }

        public override bool Connect(IDomain domain, IArea rootArea, IArea currentArea, IObject currentObject)
        {
            bool IsConnected = false;

            ConnectContent(domain, currentArea, currentObject, ref IsConnected);
            ConnectIndex(domain, currentArea, currentObject, ref IsConnected);
            ConnectGroup(domain, rootArea, ref IsConnected);

            return IsConnected;
        }

        private void ConnectContent(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            IResource Resource = ContentResource;
            IObject Object = ContentObject;
            IObjectProperty ObjectProperty = ContentObjectProperty;
            IDeclarationSource ObjectPropertyKey = ContentKey;
            IsConnected |= ContentProperty.ConnectToResourceOrObject(domain, currentArea, currentObject, ref Resource, ref Object, ref ObjectProperty, ref ObjectPropertyKey);
            ContentResource = Resource;
            ContentObject = Object;
            ContentObjectProperty = ObjectProperty;
            ContentKey = ObjectPropertyKey;

            ContentObjectProperty?.SetIsRead();
        }

        private void ConnectIndex(IDomain domain, IArea currentArea, IObject currentObject, ref bool IsConnected)
        {
            IObject Object = IndexObject;
            IObjectPropertyIndex ObjectProperty = IndexObjectProperty;
            IsConnected |= IndexProperty.ConnectToObjectIndex(domain, currentArea, currentObject, ref Object, ref ObjectProperty);
            IndexObject = Object;
            IndexObjectProperty = ObjectProperty;

            IndexObjectProperty?.SetIsReadWrite();
        }

        private void ConnectGroup(IDomain domain, IArea rootArea, ref bool IsConnected)
        {
            if (Group == null && !IsConnected) // Ensure this is checked during the second pass only, when all areas are connected.
            {
                IsConnected = true;

                if (rootArea == null)
                    throw new ParsingException(130, Source.Source, $"Radio button '{Source.Name}' not referenced in a page.");

                List<IComponentRadioButton> GroupList = new List<IComponentRadioButton>();
                rootArea.FindOtherRadioButtons(GroupName, GroupList);

                if (GroupList.Count < 2)
                    throw new ParsingException(131, Source.Source, $"Group name '{GroupName}' is only referencing one radio button.");

                int[] Indexes = new int[GroupList.Count];
                for (int i = 0; i < GroupList.Count; i++)
                {
                    Indexes[i] = GroupList[i].GroupIndex;
                    for (int j = 0; j < i; j++)
                        if (Indexes[i] == Indexes[j])
                            throw new ParsingException(132, Source.Source, $"Another radio button of the same group name '{GroupName}' is using this index.");
                }

                Group = GroupList;
            }
        }

        public override void ReportResourceKeys(IDesign design, List<string> KeyList, string styleName)
        {
            string Key = FormatStyleResourceKey(design.XamlName, styleName);
            if (!KeyList.Contains(Key))
                KeyList.Add(Key);
        }

        public static string FormatStyleResourceKey(string xamlDesignName, string styleName)
        {
            string StyleProperty = (styleName != null) ? styleName : "";
            return $"{xamlDesignName}RadioButton{StyleProperty}";
        }
    }
}
