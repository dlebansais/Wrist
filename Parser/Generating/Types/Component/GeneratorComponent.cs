using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public abstract class GeneratorComponent : IGeneratorComponent
    {
        public static Dictionary<IComponent, IGeneratorComponent> GeneratorComponentMap { get; } = new Dictionary<IComponent, IGeneratorComponent>();

        public static IGeneratorComponent Convert(IComponent component)
        {
            IGeneratorComponent Result;

            if (component is IComponentArea AsComponentArea)
                Result = new GeneratorComponentArea(AsComponentArea);
            else if (component is IComponentButton AsComponentButton)
                Result = new GeneratorComponentButton(AsComponentButton);
            else if (component is IComponentCheckBox AsComponentCheckBox)
                Result = new GeneratorComponentCheckBox(AsComponentCheckBox);
            else if (component is IComponentContainer AsComponentContainer)
                Result = new GeneratorComponentContainer(AsComponentContainer);
            else if (component is IComponentContainerList AsComponentContainerList)
                Result = new GeneratorComponentContainerList(AsComponentContainerList);
            else if (component is IComponentEdit AsComponentEdit)
                Result = new GeneratorComponentEdit(AsComponentEdit);
            else if (component is IComponentImage AsComponentImage)
                Result = new GeneratorComponentImage(AsComponentImage);
            else if (component is IComponentIndex AsComponentIndex)
                Result = new GeneratorComponentIndex(AsComponentIndex);
            else if (component is IComponentPasswordEdit AsComponentPasswordEdit)
                Result = new GeneratorComponentPasswordEdit(AsComponentPasswordEdit);
            else if (component is IComponentPopup AsComponentPopup)
                Result = new GeneratorComponentPopup(AsComponentPopup);
            else if (component is IComponentRadioButton AsComponentRadioButton)
                Result = new GeneratorComponentRadioButton(AsComponentRadioButton);
            else if (component is IComponentSelector AsComponentSelector)
                Result = new GeneratorComponentSelector(AsComponentSelector);
            else if (component is IComponentText AsComponentText)
                Result = new GeneratorComponentText(AsComponentText);
            else
                throw new InvalidOperationException();

            return Result;
        }

        public GeneratorComponent(IComponent component)
        {
            Source = component.Source;
            XamlName = component.XamlName;

            List<IComponent> IdenticalControls = new List<IComponent>();
            foreach (KeyValuePair<IComponent, IGeneratorComponent> Entry in GeneratorComponentMap)
                if (Entry.Key.XamlName == XamlName)
                    IdenticalControls.Add(Entry.Key);
            int ControlIndex = IdenticalControls.Count;

            ControlName = $"{XamlName}{ControlIndex}";

            GeneratorComponentMap.Add(component, this);
        }

        public IDeclarationSource Source { get; private set; }
        public string XamlName { get; private set; }
        public string ControlName { get; private set; }

        public abstract bool Connect(IGeneratorDomain domain);
        public abstract void Generate(IGeneratorDesign design, string styleName, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding);
        public abstract string GetStyleResourceKey(IGeneratorDesign design, string styleName);

        public string GetComponentValue(IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorResource resourceValue, IGeneratorObject objectValue, IGeneratorObjectProperty objectPropertyValue, IDeclarationSource key, bool isTwoWays)
        {
            if (resourceValue != null)
                return $"{{StaticResource {resourceValue.XamlName}}}";

            else if (objectValue != null && objectPropertyValue != null)
            {
                string Mode = isTwoWays ? ", Mode=TwoWay" : "";
                string ObjectBinding = GetObjectBinding(currentObject, objectValue, objectPropertyValue);

                if (key == null)
                    return $"{{Binding {ObjectBinding}{Mode}}}";
                else if (key.Name == GeneratorPage.CurrentPage.Name)
                    return $"{{Binding {ObjectBinding}{Mode}, Converter={{StaticResource convKeyToValue}}, ConverterParameter=page-{ParserDomain.ToKeyName(currentPage.Name)}}}";
                else
                    return $"{{Binding {ObjectBinding}{Mode}, Converter={{StaticResource convKeyToValue}}, ConverterParameter={key.Name}}}";
            }

            else
                throw new InvalidOperationException();
        }

        public string GetObjectBinding(IGeneratorObject currentObject, IGeneratorObject objectValue, IGeneratorObjectProperty objectPropertyValue)
        {
            if (objectValue == GeneratorObject.TranslationObject && objectPropertyValue == GeneratorObjectPropertyStringDictionary.StringsProperty)
                return "GetTranslation.Strings";
            else if (objectValue == currentObject)
                return objectPropertyValue.CSharpName;
            else
                return $"Get{objectValue.CSharpName}.{objectPropertyValue.CSharpName}";
        }

        public virtual bool IsReferencing(IGeneratorArea other)
        {
            return false;
        }

        public static string GetChangedHandlerName(IGeneratorObject boundObject, IGeneratorObjectProperty boundObjectProperty)
        {
            return $"OnValueChanged_{boundObject.CSharpName}_{boundObjectProperty.CSharpName}";
        }

        public static string GetLoadedHandlerName(IGeneratorObject boundObject, IGeneratorObjectProperty boundObjectProperty)
        {
            return $"OnLoaded_{boundObject.CSharpName}_{boundObjectProperty.CSharpName}";
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Source.Name}'";
        }
    }
}
