using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WpfApplication5
{
    public class Plan
    {
        public Plan()
        {
            Children = new List<Plan>();
        }

        public Plan(double left, double top, double width, double height, Brush brush, string name)
            : this()
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            Brush = brush;
            Name = name;
        }

        public Plan Parent { get; set; }
        public List<Plan> Children { get; set; }

        public List<Plan> AllParents
        {
            get
            {
                if (Parent == null)
                {
                    return new List<Plan>();
                }

                List<Plan> allParents = Parent.AllParents;
                allParents.Add(Parent);
                return allParents;
            }
        }

        public string Name { get; set; }

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Brush Brush { get; set; }

        public List<Element> Elements { get; set; }
    }

    public class Element
    {
    }
}
