using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PlansModule
{
    public class Plan
    {
        public static Random random = new Random();

        public Plan()
        {
            Children = new List<Plan>();
            Elements = new List<Element>();
        }

        public Plan(double left, double top, double width, double height, Brush brush, string name, string caption, bool addElements)
            : this()
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            Brush = brush;
            Name = name;
            Caption = caption;

            if (addElements)
            {
                for (int i = 0; i < 10; i++)
                {
                    Element element = new Element();
                    element.X = (int)(random.NextDouble() * 400);
                    element.Y = (int)(random.NextDouble() * 400);
                    Elements.Add(element);
                }
            }
        }

        public Plan Parent { get; set; }
        public List<Plan> Children { get; set; }

        public string Name { get; set; }
        public string Caption { get; set; }

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Brush Brush { get; set; }

        public List<Element> Elements { get; set; }
    }

    public class Element
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
