using System;
using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public abstract class GeneratorComponent : IGeneratorComponent
    {
        public static IGeneratorComponent Convert(IComponent component)
        {
            if (component is IComponentArea AsComponentArea)
                return new GeneratorComponentArea(AsComponentArea);
            else if (component is IComponentButton AsComponentButton)
                return new GeneratorComponentButton(AsComponentButton);
            else if (component is IComponentCheckBox AsComponentCheckBox)
                return new GeneratorComponentCheckBox(AsComponentCheckBox);
            else if (component is IComponentContainer AsComponentContainer)
                return new GeneratorComponentContainer(AsComponentContainer);
            else if (component is IComponentContainerList AsComponentContainerList)
                return new GeneratorComponentContainerList(AsComponentContainerList);
            else if (component is IComponentEdit AsComponentEdit)
                return new GeneratorComponentEdit(AsComponentEdit);
            else if (component is IComponentImage AsComponentImage)
                return new GeneratorComponentImage(AsComponentImage);
            else if (component is IComponentIndex AsComponentIndex)
                return new GeneratorComponentIndex(AsComponentIndex);
            else if (component is IComponentPasswordEdit AsComponentPasswordEdit)
                return new GeneratorComponentPasswordEdit(AsComponentPasswordEdit);
            else if (component is IComponentPopup AsComponentPopup)
                return new GeneratorComponentPopup(AsComponentPopup);
            else if (component is IComponentRadioButton AsComponentRadioButton)
                return new GeneratorComponentRadioButton(AsComponentRadioButton);
            else if (component is IComponentSelector AsComponentSelector)
                return new GeneratorComponentSelector(AsComponentSelector);
            else if (component is IComponentText AsComponentText)
                return new GeneratorComponentText(AsComponentText);
            else
                throw new InvalidOperationException();
        }

        public GeneratorComponent(IComponent component)
        {
            Source = component.Source;
            XamlName = component.XamlName;
        }

        public IDeclarationSource Source { get; private set; }
        public string XamlName { get; private set; }

        public abstract bool Connect(IGeneratorDomain domain);
        public abstract void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding);

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
                return "Translation.Strings";
            else if (objectValue == currentObject)
                return objectPropertyValue.CSharpName;
            else
                return $"{objectValue.CSharpName}.{objectPropertyValue.CSharpName}";
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
