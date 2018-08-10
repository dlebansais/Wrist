using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public abstract class GeneratorLayoutElement : IGeneratorLayoutElement
    {
        public static GeneratorLayoutElement Convert(ILayoutElement element)
        {
            if (element is Empty AsEmpty)
                return new GeneratorEmpty(AsEmpty);
            else if (element is Control AsControl)
                return new GeneratorControl(AsControl);
            else if (element is Decoration AsDecoration)
                return new GeneratorDecoration(AsDecoration);
            else if (element is Panel AsPanel)
                return GeneratorPanel.Convert(AsPanel);
            else
                throw new InvalidOperationException();
        }

        public GeneratorLayoutElement(ILayoutElement element)
        {
            Style = element.Style;
            Background = element.Background;
            Width = element.Width;
            Height = element.Height;
            Margin = element.Margin;
            HorizontalAlignment = element.HorizontalAlignment;
            VerticalAlignment = element.VerticalAlignment;

            if (DockPanel.DockTargets.ContainsKey(element))
                GeneratorDockPanel.DockTargets.Add(this, DockPanel.DockTargets[element]);

            if (Grid.ColumnTargets.ContainsKey(element))
                GeneratorGrid.ColumnTargets.Add(this, Grid.ColumnTargets[element]);

            if (Grid.RowTargets.ContainsKey(element))
                GeneratorGrid.RowTargets.Add(this, Grid.RowTargets[element]);
        }

        public string Style { get; private set; }
        public string Background { get; private set; }
        public string Width { get; private set; }
        public string Height { get; private set; }
        public string Margin { get; private set; }
        public string HorizontalAlignment { get; private set; }
        public string VerticalAlignment { get; private set; }

        public static string AttachedProperties(IGeneratorLayoutElement element)
        {
            string Result = "";

            Result += GeneratorDockPanel.AttachedDockProperty(element);
            Result += GeneratorGrid.AttachedColumnProperty(element);
            Result += GeneratorGrid.AttachedRowProperty(element);

            return Result;
        }

        protected string ElementProperties()
        {
            string Result = "";

            if (!string.IsNullOrEmpty(Background))
                Result += $" Background=\"{Background}\""; ;

            if (!string.IsNullOrEmpty(Margin))
                Result += $" Margin=\"{Margin}\""; ;

            if (!string.IsNullOrEmpty(HorizontalAlignment))
                Result += $" HorizontalAlignment=\"{HorizontalAlignment}\""; ;

            if (!string.IsNullOrEmpty(VerticalAlignment))
                Result += $" VerticalAlignment=\"{VerticalAlignment}\""; ;

            if (!string.IsNullOrEmpty(Width))
                Result += $" Width=\"{Width}\""; ;

            if (!string.IsNullOrEmpty(Height))
                Result += $" Height=\"{Height}\""; ;

            return Result;
        }

        public abstract bool Connect(IGeneratorDomain domain, IReadOnlyCollection<IGeneratorComponent> components);
        public abstract void Generate(Dictionary<IGeneratorArea, IGeneratorLayout> areaLayouts, IGeneratorDesign design, int indentation, IGeneratorPage currentPage, IGeneratorObject currentObject, IGeneratorColorTheme colorTheme, StreamWriter xamlWriter, string visibilityBinding);
    }
}
