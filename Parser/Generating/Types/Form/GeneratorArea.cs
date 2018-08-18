﻿using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorArea : IGeneratorArea
    {
        public static Dictionary<IArea, IGeneratorArea> GeneratorAreaMap { get; } = new Dictionary<IArea, IGeneratorArea>();

        public GeneratorArea(IArea area)
        {
            Name = area.Name;
            XamlName = area.XamlName;
            BaseArea = area;

            GeneratorComponentCollection ComponentList = new GeneratorComponentCollection();
            foreach (IComponent Component in area.Components)
                ComponentList.Add(GeneratorComponent.Convert(Component));

            Components = ComponentList.AsReadOnly();

            GeneratorAreaMap.Add(area, this);
        }

        private IArea BaseArea;

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public IReadOnlyCollection<IGeneratorComponent> Components { get; private set; }
        public IGeneratorObject CurrentObject { get; private set; }

        public bool Connect(IGeneratorDomain domain)
        {
            bool IsConnected = false;

            foreach (IGeneratorComponent Component in Components)
                IsConnected |= Component.Connect(domain);

            if (CurrentObject == null && BaseArea.CurrentObject != null)
            {
                IsConnected = true;

                if (GeneratorObject.GeneratorObjectMap.ContainsKey(BaseArea.CurrentObject))
                    CurrentObject = GeneratorObject.GeneratorObjectMap[BaseArea.CurrentObject];
            }

            return IsConnected;
        }

        public bool IsReferencedBy(IGeneratorArea other)
        {
            if (other == this)
                return true;

            foreach (IGeneratorComponent component in other.Components)
                if (component.IsReferencing(this))
                    return true;

            return false;
        }

        public void Generate(IGeneratorLayout layout, Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter)
        {
            layout.Generate(this, areaLayouts, design, indentation, currentPage, CurrentObject, colorTheme, xamlWriter);
        }

        public void CollectGoTo(List<IGeneratorPageNavigation> goToList, IGeneratorPage currentPage)
        {
            foreach (IGeneratorComponent Component in Components)
                if (Component is IGeneratorComponentButton AsButton)
                {
                    bool IsIncluded = false;
                    foreach (IGeneratorPageNavigation Item in goToList)
                        if (Item.IsEqual(AsButton.GoTo, currentPage))
                        {
                            IsIncluded = true;
                            break;
                        }

                    if (!IsIncluded)
                    {
                        IGeneratorPageNavigation Copy = AsButton.GoTo.CreateCopyForPage(currentPage, AsButton);
                        goToList.Add(Copy);
                    }
                }
                else if (Component is IGeneratorComponentPopup AsPopup)
                    AsPopup.Area.CollectGoTo(goToList, currentPage);
                else if (Component is IGeneratorComponentArea AsArea)
                    AsArea.Area.CollectGoTo(goToList, currentPage);
        }

        public void CollectBoundComponents(List<IGeneratorBindableComponent> boundComponentList, IGeneratorPage currentPage)
        {
            foreach (IGeneratorComponent Component in Components)
                if (Component is IGeneratorBindableComponent AsBindable)
                {
                    bool IsFound = false;
                    foreach (IGeneratorBindableComponent Item in boundComponentList)
                        if (Item.BoundObject == AsBindable.BoundObject && Item.BoundObjectProperty == AsBindable.BoundObjectProperty)
                        {
                            IsFound = true;
                            break;
                        }

                    if (!IsFound)
                        boundComponentList.Add(AsBindable);
                }
                else if (Component is IGeneratorComponentPopup AsPopup)
                    AsPopup.Area.CollectBoundComponents(boundComponentList, currentPage);
                else if (Component is IGeneratorComponentArea AsArea)
                    AsArea.Area.CollectBoundComponents(boundComponentList, currentPage);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
