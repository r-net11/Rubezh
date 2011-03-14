using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace AssadDevices
{
    public class AssadTreeBase
    {
        AssadTreeBase parent;
        public AssadTreeBase Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                if (parent != null)
                {
                    parent.Children.Add(this);
                }
            }
        }

        List<AssadTreeBase> children;
        public List<AssadTreeBase> Children
        {
            get
            {
                if (children == null)
                    children = new List<AssadTreeBase>();
                return children;
            }
            set
            {
                children = value;
            }
        }

        List<AssadTreeBase> allChildren;
        public List<AssadTreeBase> FindAllChildren()
        {
            allChildren = new List<AssadTreeBase>();
            allChildren.Add(this);
            FindChildren(this);
            return allChildren;
        }

        void FindChildren(AssadTreeBase parent)
        {
            if (parent.children != null)
                foreach (AssadTreeBase child in parent.children)
                {
                    allChildren.Add(child);
                    FindChildren(child);
                }
        }
    }
}
