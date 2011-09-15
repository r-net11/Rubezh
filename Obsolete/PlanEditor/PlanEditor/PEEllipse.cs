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
    class PEEllipse : PEShape
    {
        Ellipse ellipse;
        List<Thumb> thumbList;
        public double MinHeight
        {
            get
            {
                return ellipse.MinHeight;
            }
            set
            {
                ellipse.MinHeight = value;
            }
        }
        public double MinWidth
        {
            get
            {
                return ellipse.MinWidth;
            }
            set
            {
                ellipse.MinWidth = value;
            }
        }
        public double Height
        {
            get
            {
                return ellipse.Height;
            }
            set
            {
                ellipse.Height = value;
            }
        }

        public double Width
        {
            get
            {
                return ellipse.Width;
            }
            set
            {
                ellipse.Width = value;
            }
        }

        public double X
        {
            get
            {
                return Canvas.GetLeft(ellipse); 
            }
            set
            {
                Canvas.SetLeft(ellipse, value);
            }
        }

        public double Y
        {
            get
            {
                return Canvas.GetTop(ellipse);
            }
            set
            {
                Canvas.SetTop(ellipse, value);
            }
        }
        public PEEllipse()
            : base() 
        {
            ellipse = new Ellipse();
            MinHeight = 30;
            MinWidth = 30;
            ellipse.Fill = Brushes.Green;
            
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
            double dLeft = Canvas.GetLeft(this.ellipse);
            double dTop = Canvas.GetTop(this.ellipse);

            foreach (Thumb thumb in thumbList)
            {
                thumb.Height = 10;
                thumb.Width = 10;

                thumb.Background = Brushes.Blue;
                dLeft = Canvas.GetLeft(this.ellipse);
                dTop = Canvas.GetTop(this.ellipse);
                if (thumb.Name == "thumb1")
                {
                    Canvas.SetLeft(thumb, dLeft - 5);
                    Canvas.SetTop(thumb, dTop - 5);
                    if (canvas.Children.IndexOf(thumb) == -1) canvas.Children.Add(thumb);
                }
                if (thumb.Name == "thumb2")
                {
                    Canvas.SetLeft(thumb, dLeft+ellipse.Width - 5);
                    Canvas.SetTop(thumb, dTop - 5);
                    if (canvas.Children.IndexOf(thumb) == -1) canvas.Children.Add(thumb);
                }
                if (thumb.Name == "thumb3")
                {
                    Canvas.SetLeft(thumb, dLeft + ellipse.Width - 5);
                    Canvas.SetTop(thumb, dTop + ellipse.Height - 5);
                    if (canvas.Children.IndexOf(thumb) == -1) canvas.Children.Add(thumb);
                }
                if (thumb.Name == "thumb4")
                {
                    Canvas.SetLeft(thumb, dLeft - 5);
                    Canvas.SetTop(thumb, dTop + ellipse.Height - 5);
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
        
        public override UIElement GetShape() { return ellipse; }
    }
}
