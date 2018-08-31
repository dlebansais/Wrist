using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public abstract class GeneratorLayoutElement : IGeneratorLayoutElement
    {
        public static Dictionary<ILayoutElement, IGeneratorLayoutElement> GeneratorLayoutElementMap { get; } = new Dictionary<ILayoutElement, IGeneratorLayoutElement>();

        public static GeneratorLayoutElement Convert(ILayoutElement element)
        {
            GeneratorLayoutElement Result;

            if (element is Empty AsEmpty)
                Result = new GeneratorEmpty(AsEmpty);
            else if (element is Control AsControl)
                Result = new GeneratorControl(AsControl);
            else if (element is TextDecoration AsTextDecoration)
                Result = new GeneratorTextDecoration(AsTextDecoration);
            else if (element is Panel AsPanel)
                Result = GeneratorPanel.Convert(AsPanel);
            else
                throw new InvalidOperationException();

            GeneratorLayoutElementMap.Add(element, Result);

            return Result;
        }

        public GeneratorLayoutElement(ILayoutElement element)
        {
            Style = element.Style;
            Width = element.Width;
            Height = element.Height;
            Margin = element.Margin;
            HorizontalAlignment = element.HorizontalAlignment;
            VerticalAlignment = element.VerticalAlignment;
            BaseElement = element;

            if (DockPanel.DockTargets.ContainsKey(element))
                GeneratorDockPanel.DockTargets.Add(this, DockPanel.DockTargets[element]);

            if (Grid.ColumnTargets.ContainsKey(element))
                GeneratorGrid.ColumnTargets.Add(this, Grid.ColumnTargets[element]);

            if (Grid.ColumnSpanTargets.ContainsKey(element))
                GeneratorGrid.ColumnSpanTargets.Add(this, Grid.ColumnSpanTargets[element]);

            if (Grid.RowTargets.ContainsKey(element))
                GeneratorGrid.RowTargets.Add(this, Grid.RowTargets[element]);

            if (Grid.RowSpanTargets.ContainsKey(element))
                GeneratorGrid.RowSpanTargets.Add(this, Grid.RowSpanTargets[element]);
        }

        private ILayoutElement BaseElement;

        public string Style { get; private set; }
        public string Width { get; private set; }
        public string Height { get; private set; }
        public string Margin { get; private set; }
        public string HorizontalAlignment { get; private set; }
        public string VerticalAlignment { get; private set; }
        public IGeneratorDynamicProperty DynamicController { get; private set; }

        public virtual bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components)
        {
            bool IsConnected = false;

            if (DynamicController == null && BaseElement.DynamicController != null)
            {
                IsConnected = true;

                if (GeneratorDynamicProperty.GeneratorDynamicPropertyMap.ContainsKey(BaseElement.DynamicController))
                    DynamicController = GeneratorDynamicProperty.GeneratorDynamicPropertyMap[BaseElement.DynamicController];
            }

            return IsConnected;
        }

        protected string GetAttachedProperties()
        {
            string Result = "";

            Result += GeneratorDockPanel.AttachedDockProperty(this);
            Result += GeneratorGrid.AttachedColumnProperty(this);
            Result += GeneratorGrid.AttachedColumnSpanProperty(this);
            Result += GeneratorGrid.AttachedRowProperty(this);
            Result += GeneratorGrid.AttachedRowSpanProperty(this);

            return Result;
        }

        protected string GetElementProperties(IGeneratorPage currentPage, IGeneratorObject currentObject)
        {
            string Result = "";

            if (!string.IsNullOrEmpty(Margin))
                Result += $" Margin=\"{Margin}\"";

            if (!string.IsNullOrEmpty(HorizontalAlignment))
                Result += $" HorizontalAlignment=\"{HorizontalAlignment}\"";

            if (!string.IsNullOrEmpty(VerticalAlignment))
                Result += $" VerticalAlignment=\"{VerticalAlignment}\"";

            if (!string.IsNullOrEmpty(Width))
                Result += $" Width=\"{Width}\"";

            if (!string.IsNullOrEmpty(Height))
                Result += $" Height=\"{Height}\"";

            if (DynamicController != null)
            {
                switch (DynamicController.Result)
                {
                    case DynamicOperationResults.Boolean:
                        Result += $" IsEnabled=\"{{Binding Dynamic.{DynamicController.CSharpName}}}\" IsEnabledChanged=\"OnIsEnabledChanged\"";
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            return Result;
        }

        public abstract void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IList<IGeneratorPage> pageList, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding);
        public abstract string GetStyleResourceKey(IGeneratorDesign design);
    }
}
