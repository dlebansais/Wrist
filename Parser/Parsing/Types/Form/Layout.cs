using System.Collections.Generic;
using System.Windows.Markup;

namespace Parser
{
    [ContentProperty("Content")]
    public class Layout : ILayout
    {
        public static Layout EmptyLayout = new Layout("<empty>");

        public Layout()
        {
        }

        private Layout(string name)
        {
            Name = name;
        }

        public void SetName(string name, string xamlName, string fileName)
        {
            Name = name;
            XamlName = xamlName;
            FileName = fileName;
        }

        public string Name { get; private set; }
        public string XamlName { get; private set; }
        public string FileName { get; private set; }
        public IPanel Content { get; set; }

        public bool Connect(IDomain domain)
        {
            return false;
        }

        public virtual ILayout GetClone()
        {
            Layout Clone = new Layout();
            InitializeClone(Clone);
            return Clone;
        }

        protected virtual void InitializeClone(Layout clone)
        {
            clone.Name = Name;
            clone.XamlName = XamlName;
            clone.FileName = FileName;
            clone.Content = (IPanel)Content.GetClone();
        }

        public void ConnectComponents(IDomain domain, IDynamic currentDynamic, IReadOnlyCollection<IComponent> components)
        {
            Content.ConnectComponents(domain, currentDynamic, components);
        }

        public void ReportElementsWithAttachedProperties(List<IDockPanel> dockPanels, List<IGrid> grids)
        {
            Content.ReportElementsWithAttachedProperties(dockPanels, grids);
        }

        public void ReportResourceKeys(IDesign design, List<string> KeyList)
        {
            Content.ReportResourceKeys(design, KeyList);
        }

        public override string ToString()
        {
            return $"{GetType().Name} '{Name}' with {Content}";
        }
    }
}
