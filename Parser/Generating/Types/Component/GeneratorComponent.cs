using System;
using System.IO;
using Windows.UI.Xaml;

namespace Parser
{
    public abstract class GeneratorComponent : IGeneratorComponent
    {
        public static IGeneratorComponent Convert(IComponent component)
        {
            IComponentArea AsComponentArea;
            IComponentButton AsComponentButton;
            IComponentCheckBox AsComponentCheckBox;
            IComponentContainer AsComponentContainer;
            IComponentContainerList AsComponentContainerList;
            IComponentEdit AsComponentEdit;
            IComponentImage AsComponentImage;
            IComponentIndex AsComponentIndex;
            IComponentPasswordEdit AsComponentPasswordEdit;
            IComponentPopup AsComponentPopup;
            IComponentRadioButton AsComponentRadioButton;
            IComponentSelector AsComponentSelector;
            IComponentText AsComponentText;

            if ((AsComponentArea = component as IComponentArea) != null)
                return new GeneratorComponentArea(AsComponentArea);
            else if ((AsComponentButton = component as IComponentButton) != null)
                return new GeneratorComponentButton(AsComponentButton);
            else if ((AsComponentCheckBox = component as IComponentCheckBox) != null)
                return new GeneratorComponentCheckBox(AsComponentCheckBox);
            else if ((AsComponentContainer = component as IComponentContainer) != null)
                return new GeneratorComponentContainer(AsComponentContainer);
            else if ((AsComponentContainerList = component as IComponentContainerList) != null)
                return new GeneratorComponentContainerList(AsComponentContainerList);
            else if ((AsComponentEdit = component as IComponentEdit) != null)
                return new GeneratorComponentEdit(AsComponentEdit);
            else if ((AsComponentImage = component as IComponentImage) != null)
                return new GeneratorComponentImage(AsComponentImage);
            else if ((AsComponentIndex = component as IComponentIndex) != null)
                return new GeneratorComponentIndex(AsComponentIndex);
            else if ((AsComponentPasswordEdit = component as IComponentPasswordEdit) != null)
                return new GeneratorComponentPasswordEdit(AsComponentPasswordEdit);
            else if ((AsComponentPopup = component as IComponentPopup) != null)
                return new GeneratorComponentPopup(AsComponentPopup);
            else if ((AsComponentRadioButton = component as IComponentRadioButton) != null)
                return new GeneratorComponentRadioButton(AsComponentRadioButton);
            else if ((AsComponentSelector = component as IComponentSelector) != null)
                return new GeneratorComponentSelector(AsComponentSelector);
            else if ((AsComponentText = component as IComponentText) != null)
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
        public abstract void Generate(IGeneratorDesign design, string style, string attachedProperties, string elementProperties, TextWrapping? textWrapping, bool isHorizontalAlignmentStretch, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorScheme colorScheme, StreamWriter xamlWriter, string visibilityBinding);

        protected string GetComponentValue(IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorResource resourceValue, IGeneratorObject objectValue, IGeneratorObjectProperty objectPropertyValue, IDeclarationSource key, bool isTwoWays)
        {
            if (resourceValue != null)
                return $"{{StaticResource {resourceValue.XamlName}}}";

            else if (objectValue != null && objectPropertyValue != null)
            {
                string Mode = isTwoWays ? ", Mode=TwoWay" : "";

                if (currentObject == objectValue)
                {
                    if (key == null)
                        return $"{{Binding {objectPropertyValue.CSharpName}{Mode}}}";
                    else if (key.Name == GeneratorPage.CurrentPage.Name)
                        return $"{{Binding {objectPropertyValue.CSharpName}{Mode}, Converter={{StaticResource convKeyToValue}}, ConverterParameter={ParserDomain.ToKeyName(currentPage.Name)}}}";
                    else
                        return $"{{Binding {objectPropertyValue.CSharpName}{Mode}, Converter={{StaticResource convKeyToValue}}, ConverterParameter={key.Name}}}";
                }
                else
                {
                    if (key == null)
                        return $"{{Binding {objectValue.CSharpName}.{objectPropertyValue.CSharpName}{Mode}}}";
                    else if (key.Name == GeneratorPage.CurrentPage.Name)
                        return $"{{Binding {objectValue.CSharpName}.{objectPropertyValue.CSharpName}{Mode}, Converter={{StaticResource convKeyToValue}}, ConverterParameter={ParserDomain.ToKeyName(currentPage.Name)}}}";
                    else
                        return $"{{Binding {objectValue.CSharpName}.{objectPropertyValue.CSharpName}{Mode}, Converter={{StaticResource convKeyToValue}}, ConverterParameter={key.Name}}}";
                }
            }

            else
                throw new InvalidOperationException();
        }

        public virtual bool IsReferencing(IGeneratorArea other)
        {
            return false;
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Source.Name}'";
        }
    }
}
