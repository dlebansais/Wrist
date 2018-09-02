using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Items")]
    public class DockPanel : Panel, IDockPanel
    {
        #region Dock Attached Property
        public static Dictionary<ILayoutElement, Dock> DockTargets = new Dictionary<ILayoutElement, Dock>();

        public static object GetDock(object target)
        {
            ILayoutElement AsElement = (ILayoutElement)target;

            if (DockTargets.ContainsKey(AsElement))
                return DockTargets[AsElement];
            else
                return Dock.Left;
        }

        public static void SetDock(object target, object value)
        {
            ILayoutElement AsElement = (ILayoutElement)target;
            Dock Dock;

            if (value is string AsString)
            {
                if (AsString.ToLower() == "left")
                    Dock = Dock.Left;
                else if (AsString.ToLower() == "top")
                    Dock = Dock.Top;
                else if (AsString.ToLower() == "right")
                    Dock = Dock.Right;
                else if (AsString.ToLower() == "bottom")
                    Dock = Dock.Bottom;
                else
                    throw new ParsingException(158, AsElement.Source, $"Unknown dock value '{AsString}'.");
            }
            else
                throw new ParsingException(159, AsElement.Source, "Missing or invalid dock value.");

            if (DockTargets.ContainsKey(AsElement))
                throw new ParsingException(160, AsElement.Source, "Dock value already specified for this element.");
            else
                DockTargets.Add(AsElement, Dock);
        }

        public static void CloneDock(ILayoutElement target, ILayoutElement clone)
        {
            if (DockTargets.ContainsKey(target))
                DockTargets.Add(clone, DockTargets[target]);
        }
        #endregion

        public override ILayoutElement GetClone()
        {
            DockPanel Clone = new DockPanel();
            InitializeClone(Clone);
            return Clone;
        }

        protected override void InitializeClone(LayoutElement clone)
        {
            base.InitializeClone(clone);
        }

        public override void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids)
        {
            base.ReportElementsWithAttachedProperties(dockPanels, grids);
            dockPanels.Add(this);
        }
    }
}
