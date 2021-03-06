﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class GeneratorArea : IGeneratorArea
    {
        public static GeneratorArea EmptyArea = new GeneratorArea(Area.EmptyArea.Name);

        public static Dictionary<IArea, IGeneratorArea> GeneratorAreaMap { get; } = new Dictionary<IArea, IGeneratorArea>();

        private GeneratorArea(string name)
        {
            Name = name;
        }

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
                CurrentObject = GeneratorObject.GeneratorObjectMap[BaseArea.CurrentObject];
            }

            return IsConnected;
        }

        public bool IsReferencedBy(IGeneratorArea other)
        {
            if (other == this)
                return true;

            else if (other == EmptyArea)
                return false;

            else
            {
                foreach (IGeneratorComponent component in other.Components)
                    if (component.IsReferencing(this))
                        return true;

                return false;
            }
        }

        public void Generate(IGeneratorLayout layout, Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter)
        {
            layout.Generate(this, areaLayouts, pageList, design, indentation, currentPage, CurrentObject, colorTheme, xamlWriter);
        }

        public void CollectGoTo(List<Tuple<IGeneratorPageNavigation, IGeneratorObject, IGeneratorObjectPropertyBoolean>> goToList, IGeneratorPage currentPage)
        {
            if (this == EmptyArea)
                return;

            foreach (IGeneratorComponent Component in Components)
                if (Component is IGeneratorComponentButton AsButton)
                {
                    bool IsIncluded = false;
                    foreach (Tuple<IGeneratorPageNavigation, IGeneratorObject, IGeneratorObjectPropertyBoolean> Item in goToList)
                        if (Item.Item1.IsEqual(AsButton.GoTo, currentPage))
                        {
                            IsIncluded = true;
                            break;
                        }

                    if (!IsIncluded)
                    {
                        IGeneratorPageNavigation Copy = AsButton.GoTo.CreateCopyForPage(currentPage, AsButton);
                        IGeneratorObject ClosePopupObject = AsButton.ClosePopupObject;
                        IGeneratorObjectPropertyBoolean ClosePopupObjectProperty = AsButton.ClosePopupObjectProperty;
                        goToList.Add(new Tuple<IGeneratorPageNavigation, IGeneratorObject, IGeneratorObjectPropertyBoolean>(Copy, ClosePopupObject, ClosePopupObjectProperty));
                    }
                }
                else if (Component is IGeneratorComponentPopup AsPopup)
                    AsPopup.Area.CollectGoTo(goToList, currentPage);
                else if (Component is IGeneratorComponentArea AsArea)
                    AsArea.Area.CollectGoTo(goToList, currentPage);
                else if (Component is IGeneratorComponentContainer AsContainer)
                    AsContainer.ItemNestedArea.CollectGoTo(goToList, currentPage);
                else if (Component is IGeneratorComponentContainerList AsContainerList)
                    AsContainerList.ItemNestedArea.CollectGoTo(goToList, currentPage);
        }

        public void CollectBoundComponents(List<IGeneratorBindableComponent> boundComponentList, IGeneratorPage currentPage)
        {
            if (this == EmptyArea)
                return;

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
                else if (Component is IGeneratorComponentContainer AsContainer)
                    AsContainer.ItemNestedArea.CollectBoundComponents(boundComponentList, currentPage);
                else if (Component is IGeneratorComponentContainerList AsContainerList)
                    AsContainerList.ItemNestedArea.CollectBoundComponents(boundComponentList, currentPage);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}'";
        }
    }
}
