using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser
{
    public class ParserArea : FormParser<IArea>
    {
        public ParserArea(string folderName, string extension)
            : base(folderName, extension)
        {
        }

        public override IArea Parse(string fileName)
        {
            string Name = Path.GetFileNameWithoutExtension(fileName);
            IParsingSource Source = ParsingSource.CreateFromFileName(fileName);

            try
            {
                using (Source.Open())
                {
                    return Parse(Name, Source);
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(Source, e);
            }
        }

        private IArea Parse(string name, IParsingSource source)
        {
            IComponentCollection ComponentList;

            try
            {
                ComponentList = ParseComponents(source);
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(source, e);
            }

            return new Area(name, ParserDomain.ToXamlName(source, name, "Area"), ComponentList);
        }

        private IComponentCollection ParseComponents(IParsingSource source)
        {
            IComponentCollection ComponentList = new ComponentCollection();

            while (!source.EndOfStream)
            {
                source.ReadLine();

                IComponent NewComponent = ParseComponent(source);
                ComponentList.Add(NewComponent);
            }

            return ComponentList;
        }

        private IComponent ParseComponent(IParsingSource source)
        {
            IDeclarationSource NameSource;
            string ComponentInfo;
            ParserDomain.ParseStringPair(source, ':', out NameSource, out ComponentInfo);

            string[] SplittedInfo = ComponentInfo.Split(',');
            if (SplittedInfo.Length < 1)
                throw new ParsingException(source, "Component type expected");

            string ComponentTypeName = SplittedInfo[0].Trim();

            List<ComponentInfo> InfoList = new List<ComponentInfo>();
            for (int i = 1; i < SplittedInfo.Length; i++)
                InfoList.Add(ParseComponentInfo(source, SplittedInfo[i]));

            if (ComponentTypeName == "area")
                return ParseComponentArea(NameSource, source, InfoList);
            else if (ComponentTypeName == "button")
                return ParseComponentButton(NameSource, source, InfoList);
            else if (ComponentTypeName == "checkbox")
                return ParseComponentCheckBox(NameSource, source, InfoList);
            else if (ComponentTypeName == "text")
                return ParseComponentText(NameSource, source, InfoList);
            else if (ComponentTypeName == "image")
                return ParseComponentImage(NameSource, source, InfoList);
            else if (ComponentTypeName == "edit")
                return ParseComponentEdit(NameSource, source, InfoList);
            else if (ComponentTypeName == "password edit")
                return ParseComponentPasswordEdit(NameSource, source, InfoList);
            else if (ComponentTypeName == "popup")
                return ParseComponentPopup(NameSource, source, InfoList);
            else if (ComponentTypeName == "selector")
                return ParseComponentSelector(NameSource, source, InfoList);
            else if (ComponentTypeName == "index")
                return ParseComponentIndex(NameSource, source, InfoList);
            else if (ComponentTypeName == "container")
                return ParseComponentContainer(NameSource, source, InfoList);
            else if (ComponentTypeName == "container list")
                return ParseComponentContainerList(NameSource, source, InfoList);
            else if (ComponentTypeName == "radio button")
                return ParseComponentRadioButton(NameSource, source, InfoList);
            else
                throw new ParsingException(source, "Unknown component type");
        }

        private ComponentInfo ParseComponentInfo(IParsingSource source, string infoText)
        {
            IDeclarationSource NameSource;
            string MemberValue;
            ParserDomain.ParseStringPair(source, infoText, '=', out NameSource, out MemberValue);

            if (!MemberValue.Contains("."))
                return new ComponentInfo() { NameSource = NameSource, FixedValueSource = new DeclarationSource(MemberValue, source), ObjectSource = null, MemberSource = null, KeySource = null };

            else
            {
                IDeclarationSource ObjectSource;
                string MemberName;
                ParserDomain.ParseStringPair(source, MemberValue, '.', out ObjectSource, out MemberName);

                string Key;
                int StartIndex = MemberName.IndexOf("[");
                int EndIndex = MemberName.IndexOf("]");
                IDeclarationSource KeySource;
                if (StartIndex > 0 && EndIndex > StartIndex)
                {
                    Key = MemberName.Substring(StartIndex + 1, EndIndex - StartIndex - 1);
                    MemberName = MemberName.Substring(0, StartIndex);
                    KeySource = new DeclarationSource(Key, source);
                }
                else
                    KeySource = null;

                return new ComponentInfo() { NameSource = NameSource, FixedValueSource = null, ObjectSource = ObjectSource, MemberSource = new DeclarationSource(MemberName, source), KeySource = KeySource };
            }
        }

        private IComponentArea ParseComponentArea(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty AreaProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "name" && AreaProperty == null)
                    AreaProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "name")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (AreaProperty == null)
                throw new ParsingException(source, "Area name not specified");
            if (AreaProperty.FixedValueSource == null)
                throw new ParsingException(source, "Area can only be a static name");

            return new ComponentArea(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Area"), AreaProperty.FixedValueSource);
        }

        private IComponentButton ParseComponentButton(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty ContentProperty = null;
            IComponentEvent BeforeEvent = null;
            IComponentProperty NavigateProperty = null;
            IComponentEvent AfterEvent = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "content" && ContentProperty == null)
                    ContentProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "before" && BeforeEvent == null)
                    BeforeEvent = new ComponentEvent(Info);
                else if (Info.NameSource.Name == "goto" && NavigateProperty == null)
                    NavigateProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "after" && AfterEvent == null)
                    AfterEvent = new ComponentEvent(Info);
                else if (Info.NameSource.Name != "content" && Info.NameSource.Name != "before" && Info.NameSource.Name != "goto" && Info.NameSource.Name != "after")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (ContentProperty == null)
                throw new ParsingException(source, "Button content not specified");
            if (NavigateProperty == null)
                throw new ParsingException(source, "Button goto page name not specified");

            if (NavigateProperty.FixedValueSource == null)
                throw new ParsingException(source, "Go to page name can only be a static name");

            return new ComponentButton(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Button"), ContentProperty, BeforeEvent, NavigateProperty.FixedValueSource.Name, AfterEvent);
        }

        private IComponentCheckBox ParseComponentCheckBox(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty ContentProperty = null;
            IComponentProperty CheckedProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "content" && ContentProperty == null)
                    ContentProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "checked" && CheckedProperty == null)
                    CheckedProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "content" && Info.NameSource.Name != "checked")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (ContentProperty == null)
                throw new ParsingException(source, "CheckBox content not specified");
            if (CheckedProperty == null)
                throw new ParsingException(source, "CheckBox goto page name not specified");

            if (CheckedProperty.FixedValueSource != null)
                throw new ParsingException(source, "Checked property cannot be a static name");
            if (CheckedProperty.ObjectPropertyKey != null)
                throw new ParsingException(source, "Checked property cannot use a key");

            return new ComponentCheckBox(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "CheckBox"), ContentProperty, CheckedProperty);
        }

        private IComponentText ParseComponentText(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty TextProperty = null;
            IComponentProperty TextDecorationProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "text" && TextProperty == null)
                    TextProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "decoration" && TextDecorationProperty == null)
                    TextDecorationProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "text" && Info.NameSource.Name != "decoration")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (TextProperty == null)
                throw new ParsingException(source, "Text not specified");
            if (TextDecorationProperty != null && TextDecorationProperty.FixedValueSource == null)
                throw new ParsingException(source, "Decoration can only be a constant");

            string TextDecoration = TextDecorationProperty != null ? TextDecorationProperty.FixedValueSource.Name : null;

            if (TextDecoration != null &&
                TextDecoration != Windows.UI.Text.TextDecorations.OverLine.ToString() &&
                TextDecoration != Windows.UI.Text.TextDecorations.Strikethrough.ToString() &&
                TextDecoration != Windows.UI.Text.TextDecorations.Underline.ToString())
                throw new ParsingException(source, $"Invalid decoration for {nameSource.Name}");

            return new ComponentText(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Text"), TextProperty, TextDecoration);
        }

        private IComponentEdit ParseComponentEdit(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty TextProperty = null;
            IComponentProperty AcceptsReturnProperty = null;
            IComponentProperty TextAlignmentProperty = null;
            IComponentProperty TextWrappingProperty = null;
            IComponentProperty TextDecorationProperty = null;
            IComponentProperty HorizontalScrollBarVisibilityProperty = null;
            IComponentProperty VerticalScrollBarVisibilityProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "text" && TextProperty == null)
                    TextProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "accepts return" && AcceptsReturnProperty == null)
                    AcceptsReturnProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "alignment" && TextAlignmentProperty == null)
                    TextAlignmentProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "wrapping" && TextWrappingProperty == null)
                    TextWrappingProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "decoration" && TextDecorationProperty == null)
                    TextDecorationProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "decoration" && TextDecorationProperty == null)
                    TextDecorationProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "horizontal scrollbar" && HorizontalScrollBarVisibilityProperty == null)
                    HorizontalScrollBarVisibilityProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "vertical scrollbar" && VerticalScrollBarVisibilityProperty == null)
                    VerticalScrollBarVisibilityProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "text" && Info.NameSource.Name != "accepts return" && Info.NameSource.Name != "alignment" && Info.NameSource.Name != "wrapping" && Info.NameSource.Name != "decoration" && Info.NameSource.Name != "horizontal scrollbar" && Info.NameSource.Name != "vertical scrollbar")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (TextProperty == null)
                throw new ParsingException(source, "Text not specified");
            if (TextProperty.FixedValueSource != null || TextProperty.ObjectPropertyKey != null)
                throw new ParsingException(source, "Text must be a string property");
            if (AcceptsReturnProperty != null && AcceptsReturnProperty.FixedValueSource.Name != "Yes")
                throw new ParsingException(source, "The only valid value for the accepts return property is 'Yes'");
            if (TextAlignmentProperty != null && TextAlignmentProperty.FixedValueSource == null)
                throw new ParsingException(source, "Alignment can only be a constant");
            if (TextWrappingProperty != null && TextWrappingProperty.FixedValueSource == null)
                throw new ParsingException(source, "Wrapping can only be a constant");
            if (TextDecorationProperty != null && TextDecorationProperty.FixedValueSource == null)
                throw new ParsingException(source, "Decoration can only be a constant");
            if (HorizontalScrollBarVisibilityProperty != null && HorizontalScrollBarVisibilityProperty.FixedValueSource == null)
                throw new ParsingException(source, "Horizontal scrollbar can only be a constant");
            if (VerticalScrollBarVisibilityProperty != null && VerticalScrollBarVisibilityProperty.FixedValueSource == null)
                throw new ParsingException(source, "Vertical scrollbar can only be a constant");

            bool AcceptsReturn = (AcceptsReturnProperty != null);

            string TextAlignment = TextAlignmentProperty != null ? TextAlignmentProperty.FixedValueSource.Name : null;

            if (TextAlignment != null &&
                TextAlignment != Windows.UI.Xaml.TextAlignment.Center.ToString() &&
                TextAlignment != Windows.UI.Xaml.TextAlignment.Left.ToString() &&
                TextAlignment != Windows.UI.Xaml.TextAlignment.Right.ToString() &&
                TextAlignment != Windows.UI.Xaml.TextAlignment.Justify.ToString())
                throw new ParsingException(source, $"Invalid alignment for {nameSource.Name}");

            string TextWrapping = TextWrappingProperty != null ? TextWrappingProperty.FixedValueSource.Name : null;

            if (TextWrapping != null &&
                TextWrapping != Windows.UI.Xaml.TextWrapping.NoWrap.ToString() &&
                TextWrapping != Windows.UI.Xaml.TextWrapping.Wrap.ToString())
                throw new ParsingException(source, $"Invalid wrapping for {nameSource.Name}");

            string TextDecoration = TextDecorationProperty != null ? TextDecorationProperty.FixedValueSource.Name : null;

            if (TextDecoration != null &&
                TextDecoration != Windows.UI.Text.TextDecorations.OverLine.ToString() &&
                TextDecoration != Windows.UI.Text.TextDecorations.Strikethrough.ToString() &&
                TextDecoration != Windows.UI.Text.TextDecorations.Underline.ToString())
                throw new ParsingException(source, $"Invalid decoration for {nameSource.Name}");

            string HorizontalScrollBarVisibility = HorizontalScrollBarVisibilityProperty != null ? HorizontalScrollBarVisibilityProperty.FixedValueSource.Name : null;

            if (HorizontalScrollBarVisibility != null &&
                HorizontalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Disabled.ToString() &&
                HorizontalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto.ToString() &&
                HorizontalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Hidden.ToString() &&
                HorizontalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Visible.ToString())
                throw new ParsingException(source, $"Invalid horizontal scrollbar for {nameSource.Name}");

            string VerticalScrollBarVisibility = VerticalScrollBarVisibilityProperty != null ? VerticalScrollBarVisibilityProperty.FixedValueSource.Name : null;

            if (VerticalScrollBarVisibility != null &&
                VerticalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Disabled.ToString() &&
                VerticalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto.ToString() &&
                VerticalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Hidden.ToString() &&
                VerticalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Visible.ToString())
                throw new ParsingException(source, $"Invalid vertical scrollbar for {nameSource.Name}");

            return new ComponentEdit(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Edit"), TextProperty, AcceptsReturn, TextAlignment, TextWrapping, TextDecoration, HorizontalScrollBarVisibility, VerticalScrollBarVisibility);
        }

        private IComponentPasswordEdit ParseComponentPasswordEdit(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty TextProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "text" && TextProperty == null)
                    TextProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "text")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (TextProperty == null)
                throw new ParsingException(source, "Text not specified");
            if (TextProperty.FixedValueSource != null || TextProperty.ObjectPropertyKey != null)
                throw new ParsingException(source, "Text must be a string property");

            return new ComponentPasswordEdit(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "PasswordEdit"), TextProperty);
        }

        private IComponentImage ParseComponentImage(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty SourceProperty = null;
            IComponentProperty WidthProperty = null;
            IComponentProperty HeightProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "source" && SourceProperty == null)
                    SourceProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "width" && WidthProperty == null)
                    WidthProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "height" && HeightProperty == null)
                    HeightProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "source" && Info.NameSource.Name != "width" && Info.NameSource.Name != "height")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (SourceProperty == null)
                throw new ParsingException(source, "Source not specified");

            if (WidthProperty == null)
                throw new ParsingException(source, "Width not specified");
            if (WidthProperty.FixedValueSource == null)
                throw new ParsingException(source, "Width only be a static value");

            if (HeightProperty == null)
                throw new ParsingException(source, "Height not specified");
            if (HeightProperty.FixedValueSource == null)
                throw new ParsingException(source, "Height only be a static value");

            return new ComponentImage(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Image"), SourceProperty, WidthProperty.FixedValueSource.Name, HeightProperty.FixedValueSource.Name);
        }

        private IComponentPopup ParseComponentPopup(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty SourceProperty = null;
            IComponentProperty AreaProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "source" && SourceProperty == null)
                    SourceProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "area" && AreaProperty == null)
                    AreaProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "source" && Info.NameSource.Name != "area")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (SourceProperty == null)
                throw new ParsingException(source, "Source not specified");

            if (AreaProperty == null)
                throw new ParsingException(source, "Area not specified");

            if (AreaProperty.FixedValueSource == null)
                throw new ParsingException(source, "Area name can only be a static name");

            return new ComponentPopup(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Popup"), SourceProperty, AreaProperty.FixedValueSource);
        }

        private IComponentSelector ParseComponentSelector(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty IndexProperty = null;
            IComponentProperty ItemsProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "index" && IndexProperty == null)
                    IndexProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "items" && ItemsProperty == null)
                    ItemsProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "index" && Info.NameSource.Name != "items")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (IndexProperty == null)
                throw new ParsingException(source, "Index not specified");
            if (ItemsProperty == null)
                throw new ParsingException(source, "Items not specified");

            if (IndexProperty.FixedValueSource != null || IndexProperty.ObjectPropertyKey != null)
                throw new ParsingException(source, "Index must be an integer property");
            if (ItemsProperty.FixedValueSource != null || ItemsProperty.ObjectPropertyKey != null)
                throw new ParsingException(source, "Items must be a string property");

            return new ComponentSelector(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Selector"), IndexProperty, ItemsProperty);
        }

        private IComponentIndex ParseComponentIndex(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty IndexProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "index" && IndexProperty == null)
                    IndexProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "index")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (IndexProperty == null)
                throw new ParsingException(source, "Index not specified");

            if (IndexProperty.FixedValueSource != null || IndexProperty.ObjectPropertyKey != null)
                throw new ParsingException(source, "Index must be an integer, state or boolean property");

            return new ComponentIndex(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Index"), IndexProperty);
        }

        private IComponentContainer ParseComponentContainer(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty ItemProperty = null;
            IComponentProperty AreaProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "item" && ItemProperty == null)
                    ItemProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "area" && AreaProperty == null)
                    AreaProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "item" && Info.NameSource.Name != "area")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (ItemProperty == null)
                throw new ParsingException(source, "Item not specified");
            if (AreaProperty == null)
                throw new ParsingException(source, "Area not specified");
            if (AreaProperty.FixedValueSource == null)
                throw new ParsingException(source, "Area name can only be a static name");

            return new ComponentContainer(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Container"), ItemProperty, AreaProperty.FixedValueSource);
        }

        private IComponentContainerList ParseComponentContainerList(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty ItemListProperty = null;
            IComponentProperty AreaProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "item list" && ItemListProperty == null)
                    ItemListProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "area" && AreaProperty == null)
                    AreaProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "item list" && Info.NameSource.Name != "area")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (ItemListProperty == null)
                throw new ParsingException(source, "Item list not specified");

            if (AreaProperty == null)
                throw new ParsingException(source, "Area not specified");
            if (AreaProperty.FixedValueSource == null)
                throw new ParsingException(source, "Area name can only be a static name");

            return new ComponentContainerList(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "ContainerList"), ItemListProperty, AreaProperty.FixedValueSource);
        }

        private IComponentRadioButton ParseComponentRadioButton(IDeclarationSource nameSource, IParsingSource source, List<ComponentInfo> infoList)
        {
            IComponentProperty ContentProperty = null;
            IComponentProperty IndexProperty = null;
            IComponentProperty GroupNameProperty = null;
            IComponentProperty GroupIndexProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "content" && ContentProperty == null)
                    ContentProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "index" && IndexProperty == null)
                    IndexProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "group name" && GroupNameProperty == null)
                    GroupNameProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "group index" && GroupIndexProperty == null)
                    GroupIndexProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "content" && Info.NameSource.Name != "index" && Info.NameSource.Name != "group name" && Info.NameSource.Name != "group index")
                    throw new ParsingException(source, $"Unknown token {Info.NameSource.Name}");
                else
                    throw new ParsingException(source, $"Repeated: {Info.NameSource.Name}");

            if (ContentProperty == null)
                throw new ParsingException(source, "CheckBox content not specified");

            if (IndexProperty == null)
                throw new ParsingException(source, "Index not specified");
            if (IndexProperty.FixedValueSource != null || IndexProperty.ObjectPropertyKey != null)
                throw new ParsingException(source, "Index must be an integer, state or boolean property");

            if (GroupNameProperty == null)
                throw new ParsingException(source, "Group name not specified");
            if (GroupNameProperty.FixedValueSource == null)
                throw new ParsingException(source, "Group name can only be a static name");

            if (GroupIndexProperty == null)
                throw new ParsingException(source, "Group index not specified");
            if (GroupIndexProperty.FixedValueSource == null)
                throw new ParsingException(source, "Group index can only be a static value");

            int GroupIndex;
            if (!int.TryParse(GroupIndexProperty.FixedValueSource.Name, out GroupIndex))
                throw new ParsingException(source, "Group index must be an integer");

            return new ComponentRadioButton(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "RadioButton"), ContentProperty, IndexProperty, GroupNameProperty.FixedValueSource.Name, GroupIndex);
        }
    }
}
