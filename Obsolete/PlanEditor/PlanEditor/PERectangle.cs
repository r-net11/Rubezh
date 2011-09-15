using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using PlanEditor.Basic;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace PlanEditor
{
    class PERectangle : PEShape
    {
        Rectangle rect;
        List<Thumb> thumbList;
        public double MinHeight
        {
            get
            {
                return rect.MinHeight;
            }
            set
            {
                rect.MinHeight = value;
            }
        }
        public double MinWidth
        {
            get
            {
                return rect.MinWidth;
            }
            set
            {
                rect.MinWidth = value;
            }
        }
        public double Height
        {
            get
            {
                return rect.Height;
            }
            set
            {
                rect.Height = value;
            }
        }

        public double Width
        {
            get
            {
                return rect.Width;
            }
            set
            {
                rect.Width = value;
            }
        }

        public double X
        {
            get
            {
                return Canvas.GetLeft(rect); 
            }
            set
            {
                Canvas.SetLeft(rect, value);
            }
        }

        public double Y
        {
            get
            {
                return Canvas.GetTop(rect);
            }
            set
            {
                Canvas.SetTop(rect, value);
            }
        }
        public PERectangle()
            : base() 
        {
            rect = new Rectangle();
            MinHeight = 30;
            MinWidth = 30;
            rect.Fill = Brushes.Blue;
            
            Height = 100;
            Width = 100;
            thumbList = new List<Thumb>();
            for (int i = 0; i < 4; i++)
            {
                Thumb thumbTmp = new Thumb();
                string s = "thumb" + (i + 1).ToString();
                try
                {
                    thumbTmp.Name = s;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                thumbList.Add(thumbTmp);
            }
        }
        public void SetActive(Canvas canvas)
        {
            active = true;
            double dLeft = Canvas.GetLeft(this.rect);
            double dTop = Canvas.GetTop(this.rect);

            foreach (Thumb thumb in thumbList)
            {
                thumb.Height = 10;
                thumb.Width = 10;

                thumb.Background = Brushes.Blue;
                dLeft = Canvas.GetLeft(this.rect);
                dTop = Canvas.GetTop(this.rect);
                if (thumb.Name == "thumb1")
                {
                    Canvas.SetLeft(thumb, dLeft - 5);
                    Canvas.SetTop(thumb, dTop - 5);
                    if (canvas.Children.IndexOf(thumb) == -1) canvas.Children.Add(thumb);
                }
                if (thumb.Name == "thumb2")
                {
                    Canvas.SetLeft(thumb, dLeft+rect.Width - 5);
                    Canvas.SetTop(thumb, dTop - 5);
                    if (canvas.Children.IndexOf(thumb) == -1) canvas.Children.Add(thumb);
                }
                if (thumb.Name == "thumb3")
                {
                    Canvas.SetLeft(thumb, dLeft + rect.Width - 5);
                    Canvas.SetTop(thumb, dTop + rect.Height - 5);
                    if (canvas.Children.IndexOf(thumb) == -1) canvas.Children.Add(thumb);
                }
                if (thumb.Name == "thumb4")
                {
                    Canvas.SetLeft(thumb, dLeft - 5);
                    Canvas.SetTop(thumb, dTop + rect.Height - 5);
                    if (canvas.Children.IndexOf(thumb) == -1) canvas.Children.Add(thumb);
                }
            }
        }
        public void SetDeactive(Canvas canvas)
        {
            active = false;
            foreach (Thumb thumb in thumbList)
            {
                if (canvas.Children.IndexOf(thumb) != -1) canvas.Children.Remove(thumb);
            }

        }

        public override UIElement GetShape() { return rect; }
    }
}
