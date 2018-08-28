using System;
using System.Collections.Generic;
using System.IO;

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
            IParsingSourceStream SourceStream = ParsingSourceStream.CreateFromFileName(fileName);

            try
            {
                using (SourceStream.Open())
                {
                    return Parse(Name, SourceStream);
                }
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(24, SourceStream, e);
            }
        }

        private IArea Parse(string name, IParsingSourceStream sourceStream)
        {
            IComponentCollection ComponentList;

            try
            {
                ComponentList = ParseComponents(sourceStream);
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParsingException(24, sourceStream, e);
            }

            return new Area(name, ParserDomain.ToXamlName(sourceStream, name, "Area"), ComponentList);
        }

        private IComponentCollection ParseComponents(IParsingSourceStream sourceStream)
        {
            IComponentCollection ComponentList = new ComponentCollection();

            while (!sourceStream.EndOfStream)
            {
                sourceStream.ReadLine();

                IComponent NewComponent = ParseComponent(sourceStream);
                ComponentList.Add(NewComponent);
            }

            return ComponentList;
        }

        private IComponent ParseComponent(IParsingSourceStream sourceStream)
        {
            IDeclarationSource NameSource;
            string ComponentInfo;
            ParserDomain.ParseStringPair(sourceStream, ':', out NameSource, out ComponentInfo);

            string[] SplittedInfo = ComponentInfo.Split(',');
            if (SplittedInfo.Length < 1)
                throw new ParsingException(25, sourceStream, "Component type expected.");

            string ComponentTypeName = SplittedInfo[0].Trim();
            if (string.IsNullOrEmpty(ComponentTypeName))
                throw new ParsingException(25, sourceStream, "Component type expected.");

            List<ComponentInfo> InfoList = new List<ComponentInfo>();
            for (int i = 1; i < SplittedInfo.Length; i++)
                InfoList.Add(ParseComponentInfo(sourceStream, SplittedInfo[i]));

            if (ComponentTypeName == "area")
                return ParseComponentArea(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "button")
                return ParseComponentButton(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "checkbox")
                return ParseComponentCheckBox(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "text")
                return ParseComponentText(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "image")
                return ParseComponentImage(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "edit")
                return ParseComponentEdit(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "password edit")
                return ParseComponentPasswordEdit(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "popup")
                return ParseComponentPopup(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "selector")
                return ParseComponentSelector(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "index")
                return ParseComponentIndex(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "container")
                return ParseComponentContainer(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "container list")
                return ParseComponentContainerList(NameSource, sourceStream, InfoList);
            else if (ComponentTypeName == "radio button")
                return ParseComponentRadioButton(NameSource, sourceStream, InfoList);
            else
                throw new ParsingException(26, sourceStream, $"Unknown component type '{ComponentTypeName}'.");
        }

        private ComponentInfo ParseComponentInfo(IParsingSourceStream sourceStream, string infoText)
        {
            IDeclarationSource NameSource;
            string MemberValue;
            ParserDomain.ParseStringPair(sourceStream, infoText, '=', out NameSource, out MemberValue);

            if (!MemberValue.Contains("."))
                return new ComponentInfo() { NameSource = NameSource, FixedValueSource = new DeclarationSource(MemberValue, sourceStream), ObjectSource = null, MemberSource = null, KeySource = null };

            else
            {
                IDeclarationSource ObjectSource;
                string MemberName;
                ParserDomain.ParseStringPair(sourceStream, MemberValue, '.', out ObjectSource, out MemberName);

                string Key;
                int StartIndex = MemberName.IndexOf("[");
                int EndIndex = MemberName.IndexOf("]");
                IDeclarationSource KeySource;
                if (StartIndex > 0 && EndIndex > StartIndex)
                {
                    Key = MemberName.Substring(StartIndex + 1, EndIndex - StartIndex - 1);
                    MemberName = MemberName.Substring(0, StartIndex);
                    KeySource = new DeclarationSource(Key, sourceStream);
                }
                else
                    KeySource = null;

                return new ComponentInfo() { NameSource = NameSource, FixedValueSource = null, ObjectSource = ObjectSource, MemberSource = new DeclarationSource(MemberName, sourceStream), KeySource = KeySource };
            }
        }

        private IComponentArea ParseComponentArea(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty AreaProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "name" && AreaProperty == null)
                    AreaProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "name")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (AreaProperty == null)
                throw new ParsingException(29, sourceStream, "Area name not specified.");
            if (AreaProperty.FixedValueSource == null)
                throw new ParsingException(30, sourceStream, "Area can only be a static name.");

            return new ComponentArea(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Area"), AreaProperty.FixedValueSource);
        }

        private IComponentButton ParseComponentButton(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty ContentProperty = null;
            IComponentEvent BeforeEvent = null;
            IComponentProperty NavigateProperty = null;
            IComponentEvent AfterEvent = null;
            IComponentProperty ClosePopupProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "content" && ContentProperty == null)
                    ContentProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "before" && BeforeEvent == null)
                    BeforeEvent = new ComponentEvent(Info);
                else if (Info.NameSource.Name == "goto" && NavigateProperty == null)
                    NavigateProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "after" && AfterEvent == null)
                    AfterEvent = new ComponentEvent(Info);
                else if (Info.NameSource.Name == "close popup" && ClosePopupProperty == null)
                    ClosePopupProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "content" && Info.NameSource.Name != "before" && Info.NameSource.Name != "goto" && Info.NameSource.Name != "after" && Info.NameSource.Name != "close popup")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (ContentProperty == null)
                throw new ParsingException(31, sourceStream, "Button content not specified.");
            if (NavigateProperty == null)
                throw new ParsingException(32, sourceStream, "Button goto page name not specified.");

            if (NavigateProperty.FixedValueSource == null)
                throw new ParsingException(33, sourceStream, "Go to page name can only be a static name.");

            if (ClosePopupProperty != null && ClosePopupProperty.FixedValueSource != null)
                throw new ParsingException(219, sourceStream, "Close popup can only be a property.");

            return new ComponentButton(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Button"), ContentProperty, BeforeEvent, NavigateProperty.FixedValueSource.Name, AfterEvent, ClosePopupProperty);
        }

        private IComponentCheckBox ParseComponentCheckBox(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty ContentProperty = null;
            IComponentProperty CheckedProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "content" && ContentProperty == null)
                    ContentProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "checked" && CheckedProperty == null)
                    CheckedProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "content" && Info.NameSource.Name != "checked")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (ContentProperty == null)
                throw new ParsingException(34, sourceStream, "CheckBox content not specified.");
            if (CheckedProperty == null)
                throw new ParsingException(35, sourceStream, "CheckBox checked property not specified.");

            if (CheckedProperty.FixedValueSource != null)
                throw new ParsingException(36, sourceStream, "Checked property cannot be a static name.");
            if (CheckedProperty.ObjectPropertyKey != null)
                throw new ParsingException(37, sourceStream, "Checked property cannot use a key.");

            return new ComponentCheckBox(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "CheckBox"), ContentProperty, CheckedProperty);
        }

        private IComponentText ParseComponentText(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty TextProperty = null;
            IComponentProperty TextDecorationProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "text" && TextProperty == null)
                    TextProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "decoration" && TextDecorationProperty == null)
                    TextDecorationProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "text" && Info.NameSource.Name != "decoration")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (TextProperty == null)
                throw new ParsingException(38, sourceStream, "Text not specified.");
            if (TextDecorationProperty != null && TextDecorationProperty.FixedValueSource == null)
                throw new ParsingException(39, sourceStream, "Decoration can only be a constant.");

            string TextDecoration = TextDecorationProperty != null ? TextDecorationProperty.FixedValueSource.Name : null;

            if (TextDecoration != null &&
                TextDecoration != Windows.UI.Text.TextDecorations.OverLine.ToString() &&
                TextDecoration != Windows.UI.Text.TextDecorations.Strikethrough.ToString() &&
                TextDecoration != Windows.UI.Text.TextDecorations.Underline.ToString())
                throw new ParsingException(40, sourceStream, $"Invalid decoration for '{nameSource.Name}'.");

            return new ComponentText(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Text"), TextProperty, TextDecoration);
        }

        private IComponentEdit ParseComponentEdit(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty TextProperty = null;
            IComponentProperty AcceptsReturnProperty = null;
            IComponentProperty TextDecorationProperty = null;
            IComponentProperty HorizontalScrollBarVisibilityProperty = null;
            IComponentProperty VerticalScrollBarVisibilityProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "text" && TextProperty == null)
                    TextProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "accepts return" && AcceptsReturnProperty == null)
                    AcceptsReturnProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "decoration" && TextDecorationProperty == null)
                    TextDecorationProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "decoration" && TextDecorationProperty == null)
                    TextDecorationProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "horizontal scrollbar" && HorizontalScrollBarVisibilityProperty == null)
                    HorizontalScrollBarVisibilityProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "vertical scrollbar" && VerticalScrollBarVisibilityProperty == null)
                    VerticalScrollBarVisibilityProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "text" && Info.NameSource.Name != "accepts return" && Info.NameSource.Name != "alignment" && Info.NameSource.Name != "wrapping" && Info.NameSource.Name != "decoration" && Info.NameSource.Name != "horizontal scrollbar" && Info.NameSource.Name != "vertical scrollbar")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (TextProperty == null)
                throw new ParsingException(41, sourceStream, "Text not specified.");
            if (TextProperty.FixedValueSource != null || TextProperty.ObjectPropertyKey != null)
                throw new ParsingException(42, sourceStream, "Text must be a string property.");
            if (AcceptsReturnProperty != null && AcceptsReturnProperty.FixedValueSource.Name != "Yes")
                throw new ParsingException(43, sourceStream, "The only valid value for the 'accepts return' property is 'Yes'.");
            if (TextDecorationProperty != null && TextDecorationProperty.FixedValueSource == null)
                throw new ParsingException(44, sourceStream, "Decoration can only be a constant.");
            if (HorizontalScrollBarVisibilityProperty != null && HorizontalScrollBarVisibilityProperty.FixedValueSource == null)
                throw new ParsingException(45, sourceStream, "Horizontal scrollbar can only be a constant.");
            if (VerticalScrollBarVisibilityProperty != null && VerticalScrollBarVisibilityProperty.FixedValueSource == null)
                throw new ParsingException(46, sourceStream, "Vertical scrollbar can only be a constant.");

            bool AcceptsReturn = (AcceptsReturnProperty != null);

            string TextDecoration = TextDecorationProperty != null ? TextDecorationProperty.FixedValueSource.Name : null;

            if (TextDecoration != null &&
                TextDecoration != Windows.UI.Text.TextDecorations.OverLine.ToString() &&
                TextDecoration != Windows.UI.Text.TextDecorations.Strikethrough.ToString() &&
                TextDecoration != Windows.UI.Text.TextDecorations.Underline.ToString())
                throw new ParsingException(47, sourceStream, $"Invalid decoration for '{nameSource.Name}'.");

            string HorizontalScrollBarVisibility = HorizontalScrollBarVisibilityProperty != null ? HorizontalScrollBarVisibilityProperty.FixedValueSource.Name : null;

            if (HorizontalScrollBarVisibility != null &&
                HorizontalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Disabled.ToString() &&
                HorizontalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto.ToString() &&
                HorizontalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Hidden.ToString() &&
                HorizontalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Visible.ToString())
                throw new ParsingException(48, sourceStream, $"Invalid horizontal scrollbar for '{nameSource.Name}'.");

            string VerticalScrollBarVisibility = VerticalScrollBarVisibilityProperty != null ? VerticalScrollBarVisibilityProperty.FixedValueSource.Name : null;

            if (VerticalScrollBarVisibility != null &&
                VerticalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Disabled.ToString() &&
                VerticalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto.ToString() &&
                VerticalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Hidden.ToString() &&
                VerticalScrollBarVisibility != Windows.UI.Xaml.Controls.ScrollBarVisibility.Visible.ToString())
                throw new ParsingException(49, sourceStream, $"Invalid vertical scrollbar for '{nameSource.Name}'.");

            return new ComponentEdit(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Edit"), TextProperty, AcceptsReturn, TextDecoration, HorizontalScrollBarVisibility, VerticalScrollBarVisibility);
        }

        private IComponentPasswordEdit ParseComponentPasswordEdit(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty TextProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "text" && TextProperty == null)
                    TextProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "text")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (TextProperty == null)
                throw new ParsingException(50, sourceStream, "Text not specified.");
            if (TextProperty.FixedValueSource != null || TextProperty.ObjectPropertyKey != null)
                throw new ParsingException(51, sourceStream, "Text must be a string property.");

            return new ComponentPasswordEdit(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "PasswordEdit"), TextProperty);
        }

        private IComponentImage ParseComponentImage(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
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
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (SourceProperty == null)
                throw new ParsingException(52, sourceStream, "Source not specified.");

            bool IsResourceWidth;
            double WidthValue;
            if (WidthProperty != null)
            {
                if (WidthProperty.FixedValueSource == null)
                    throw new ParsingException(54, sourceStream, "Width can only be a static value.");

                string ImageWidth = WidthProperty.FixedValueSource.Name;
                if (ImageWidth == "resource")
                {
                    IsResourceWidth = true;
                    WidthValue = double.NaN;
                }
                else if (ParserDomain.TryParseDouble(ImageWidth, out WidthValue))
                    IsResourceWidth = false;
                else
                    throw new ParsingException(128, WidthProperty.FixedValueSource.Source, $"'{ImageWidth}' not parsed as a width.");
            }
            else
            {
                IsResourceWidth = false;
                WidthValue = double.NaN;
            }

            bool IsResourceHeight;
            double HeightValue;
            if (HeightProperty != null)
            {
                if (HeightProperty.FixedValueSource == null)
                    throw new ParsingException(56, sourceStream, "Height can only be a static value.");

                string ImageHeight = HeightProperty.FixedValueSource.Name;
                if (ImageHeight == "resource")
                {
                    IsResourceHeight = true;
                    HeightValue = double.NaN;
                }
                else if (ParserDomain.TryParseDouble(ImageHeight, out HeightValue))
                    IsResourceHeight = false;
                else
                    throw new ParsingException(129, HeightProperty.FixedValueSource.Source, $"'{ImageHeight}' not parsed as a height.");
            }
            else
            {
                IsResourceHeight = false;
                HeightValue = double.NaN;
            }

            return new ComponentImage(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Image"), SourceProperty, IsResourceWidth, WidthValue, IsResourceHeight, HeightValue);
        }

        private IComponentPopup ParseComponentPopup(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty SourceProperty = null;
            IComponentProperty SourcePressedProperty = null;
            IComponentProperty AreaProperty = null;
            IComponentProperty WidthProperty = null;
            IComponentProperty HeightProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "source" && SourceProperty == null)
                    SourceProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "source pressed" && SourcePressedProperty == null)
                    SourcePressedProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "area" && AreaProperty == null)
                    AreaProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "width" && WidthProperty == null)
                    WidthProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "height" && HeightProperty == null)
                    HeightProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "source" && Info.NameSource.Name != "source pressed" && Info.NameSource.Name != "area" && Info.NameSource.Name != "width" && Info.NameSource.Name != "height")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (SourceProperty == null)
                throw new ParsingException(57, sourceStream, "Source not specified.");
            if (SourceProperty.FixedValueSource == null)
                throw new ParsingException(186, sourceStream, "Source can only be a static name.");

            if (SourcePressedProperty != null && SourcePressedProperty.FixedValueSource == null)
                throw new ParsingException(186, sourceStream, "Source pressed can only be a static name.");

            if (AreaProperty == null)
                throw new ParsingException(58, sourceStream, "Area not specified.");
            if (AreaProperty.FixedValueSource == null)
                throw new ParsingException(59, sourceStream, "Area name can only be a static name.");

            double WidthValue;
            if (WidthProperty != null)
            {
                if (WidthProperty.FixedValueSource == null)
                    throw new ParsingException(54, sourceStream, "Width can only be a static value.");

                string ImageWidth = WidthProperty.FixedValueSource.Name;
                if (!ParserDomain.TryParseDouble(ImageWidth, out WidthValue))
                    throw new ParsingException(128, WidthProperty.FixedValueSource.Source, $"'{ImageWidth}' not parsed as a width.");
            }
            else
                WidthValue = double.NaN;

            double HeightValue;
            if (HeightProperty != null)
            {
                if (HeightProperty.FixedValueSource == null)
                    throw new ParsingException(56, sourceStream, "Height can only be a static value.");

                string ImageHeight = HeightProperty.FixedValueSource.Name;
                if (!ParserDomain.TryParseDouble(ImageHeight, out HeightValue))
                    throw new ParsingException(129, HeightProperty.FixedValueSource.Source, $"'{ImageHeight}' not parsed as a height.");
            }
            else
                HeightValue = double.NaN;

            return new ComponentPopup(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Popup"), SourceProperty, SourcePressedProperty, AreaProperty.FixedValueSource, WidthValue, HeightValue);
        }

        private IComponentSelector ParseComponentSelector(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty IndexProperty = null;
            IComponentProperty ItemsProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "index" && IndexProperty == null)
                    IndexProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "items" && ItemsProperty == null)
                    ItemsProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "index" && Info.NameSource.Name != "items")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (IndexProperty == null)
                throw new ParsingException(60, sourceStream, "Index not specified.");
            if (IndexProperty.FixedValueSource != null || IndexProperty.ObjectPropertyKey != null)
                throw new ParsingException(61, sourceStream, "Index must be an integer property.");

            if (ItemsProperty == null)
                throw new ParsingException(62, sourceStream, "Items not specified.");
            if (ItemsProperty.FixedValueSource != null || ItemsProperty.ObjectPropertyKey != null)
                throw new ParsingException(63, sourceStream, "Items must be a list property.");

            return new ComponentSelector(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Selector"), IndexProperty, ItemsProperty);
        }

        private IComponentIndex ParseComponentIndex(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty IndexProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "index" && IndexProperty == null)
                    IndexProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "index")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (IndexProperty == null)
                throw new ParsingException(64, sourceStream, "Index not specified.");
            if (IndexProperty.FixedValueSource != null || IndexProperty.ObjectPropertyKey != null)
                throw new ParsingException(65, sourceStream, "Index must be an integer, state or boolean property.");

            return new ComponentIndex(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Index"), IndexProperty);
        }

        private IComponentContainer ParseComponentContainer(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty ItemProperty = null;
            IComponentProperty AreaProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "item" && ItemProperty == null)
                    ItemProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "area" && AreaProperty == null)
                    AreaProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "item" && Info.NameSource.Name != "area")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (ItemProperty == null)
                throw new ParsingException(66, sourceStream, "Item not specified.");

            if (AreaProperty == null)
                throw new ParsingException(67, sourceStream, "Area not specified.");
            if (AreaProperty.FixedValueSource == null)
                throw new ParsingException(68, sourceStream, "Area name can only be a static name.");

            return new ComponentContainer(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "Container"), ItemProperty, AreaProperty.FixedValueSource);
        }

        private IComponentContainerList ParseComponentContainerList(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
        {
            IComponentProperty ItemListProperty = null;
            IComponentProperty AreaProperty = null;

            foreach (ComponentInfo Info in infoList)
                if (Info.NameSource.Name == "items" && ItemListProperty == null)
                    ItemListProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name == "area" && AreaProperty == null)
                    AreaProperty = new ComponentProperty(Info);
                else if (Info.NameSource.Name != "items" && Info.NameSource.Name != "area")
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (ItemListProperty == null)
                throw new ParsingException(69, sourceStream, "Items not specified.");

            if (AreaProperty == null)
                throw new ParsingException(70, sourceStream, "Area not specified.");
            if (AreaProperty.FixedValueSource == null)
                throw new ParsingException(71, sourceStream, "Area name can only be a static name.");

            return new ComponentContainerList(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "ContainerList"), ItemListProperty, AreaProperty.FixedValueSource);
        }

        private IComponentRadioButton ParseComponentRadioButton(IDeclarationSource nameSource, IParsingSourceStream sourceStream, List<ComponentInfo> infoList)
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
                    throw new ParsingException(27, sourceStream, $"Unknown token '{Info.NameSource.Name}'.");
                else
                    throw new ParsingException(28, sourceStream, $"'{Info.NameSource.Name}' is repeated.");

            if (ContentProperty == null)
                throw new ParsingException(72, sourceStream, "Radio button content not specified.");

            if (IndexProperty == null)
                throw new ParsingException(73, sourceStream, "Index not specified.");
            if (IndexProperty.FixedValueSource != null || IndexProperty.ObjectPropertyKey != null)
                throw new ParsingException(74, sourceStream, "Index must be an integer, state or boolean property.");

            if (GroupNameProperty == null)
                throw new ParsingException(75, sourceStream, "Group name not specified.");
            if (GroupNameProperty.FixedValueSource == null)
                throw new ParsingException(76, sourceStream, "Group name can only be a static name.");

            if (GroupIndexProperty == null)
                throw new ParsingException(77, sourceStream, "Group index not specified.");

            int GroupIndex;
            if (GroupIndexProperty.FixedValueSource == null || !int.TryParse(GroupIndexProperty.FixedValueSource.Name, out GroupIndex))
                throw new ParsingException(78, sourceStream, "Group index must be an integer constant.");

            return new ComponentRadioButton(nameSource, ParserDomain.ToXamlName(nameSource.Source, nameSource.Name, "RadioButton"), ContentProperty, IndexProperty, GroupNameProperty.FixedValueSource.Name, GroupIndex);
        }
    }
}
