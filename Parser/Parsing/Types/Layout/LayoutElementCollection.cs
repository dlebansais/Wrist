using System.Collections.Generic;

namespace Parser
{
    public class LayoutElementCollection : List<ILayoutElement>, ILayoutElementCollection
    {
        public virtual ILayoutElementCollection GetClone()
        {
            LayoutElementCollection Clone = new LayoutElementCollection();
            InitializeClone(Clone);
            return Clone;
        }

        protected virtual void InitializeClone(LayoutElementCollection clone)
        {
            foreach (ILayoutElement Item in this)
                clone.Add(Item.GetClone());
        }
    }
}
