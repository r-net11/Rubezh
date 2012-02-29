using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrastructure;

namespace PlansModule.Designer
{
    public class ResizeChromePolyline : Canvas, IResizeChromeBase
    {
        static ResizeChromePolyline()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChromePolyline), new FrameworkPropertyMetadata(typeof(ResizeChromePolyline)));
        }

        DesignerItem DesignerItem;

        DesignerCanvas DesignerCanvas
        {
            get { return DesignerItem.DesignerCanvas; }
        }

        Polyline Polyline
        {
            get { return DesignerItem.Content as Polyline; }
        }

        double ZoomFactor
        {
            get { return DesignerCanvas.PlanDesignerViewModel.Zoom; }
        }

        List<ThumbPolygon> thumbs = new List<ThumbPolygon>();

        public ResizeChromePolyline(DesignerItem designerItem)
        {
            DesignerItem = designerItem;
            designerItem.ResizeChromeBase = this;
            Initialize();
        }

        public void Initialize()
        {
            ArrangeSize();
            AddThumbs();
        }

        void AddThumbs()
        {
            this.Children.Clear();
            thumbs.Clear();

            foreach (var point in Polyline.Points)
            {
                var thumb = new ThumbPolygon()
                {
                    Width = 7 / ZoomFactor,
                    Height = 7 / ZoomFactor,
                    Margin = new Thickness(-3.5 / ZoomFactor, -3.5 / ZoomFactor, 0, 0),
                    BorderThickness = new Thickness(0),
                    Focusable = true
                };
                Canvas.SetLeft(thumb, point.X);
                Canvas.SetTop(thumb, point.Y);
                thumb.DragStarted += new DragStartedEventHandler(thumb_DragStarted);
                thumb.DragDelta += new DragDeltaEventHandler(_thumb_DragDelta);
                thumb.DragCompleted += new DragCompletedEventHandler(thumb_DragCompleted);
                thumb.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(thumb_PreviewKeyDown);
                thumbs.Add(thumb);
                this.Children.Add(thumb);
            }
        }

        void thumb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var thumb = sender as ThumbPolygon;
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                thumbs.Remove(thumb);
                SavePolygonPointsFromThumb();
                ServiceFactory.SaveService.PlansChanged = true;
                Initialize();
                e.Handled = true;
            }
        }

        void thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            SavePolygonPointsFromThumb();
            ArrangeSize();
            DesignerCanvas.BeginChange();
        }

        void thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            DesignerCanvas.EndChange();
        }

        private void _thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var currentThumb = sender as ThumbPolygon;

            double left = Canvas.GetLeft(DesignerItem) + Canvas.GetLeft(currentThumb) + e.HorizontalChange;
            double top = Canvas.GetTop(DesignerItem) + Canvas.GetTop(currentThumb) + e.VerticalChange;

            if (left < 0)
                left = 0;
            if (top < 0)
                top = 0;
            if (left > DesignerCanvas.Width)
                left = DesignerCanvas.Width;
            if (top > DesignerCanvas.Height)
                top = DesignerCanvas.Height;

            left -= Canvas.GetLeft(DesignerItem);
            top -= Canvas.GetTop(DesignerItem);

            Canvas.SetLeft(currentThumb, left);
            Canvas.SetTop(currentThumb, top);

            ArrangeSize();
            SavePolygonPointsFromThumb();
            ServiceFactory.SaveService.PlansChanged = true;
        }

        void SavePolygonPointsFromThumb()
        {
            Polyline.Points.Clear();
            foreach (var thumb in thumbs)
            {
                Polyline.Points.Add(new Point(Canvas.GetLeft(thumb), Canvas.GetTop(thumb)));
            }
        }

        public void ArrangeSize()
        {
            double minLeft = double.MaxValue;
            double minTop = double.MaxValue;
            double maxLeft = 0;
            double maxTop = 0;
            foreach (var point in Polyline.Points)
            {
                minLeft = Math.Min(point.X, minLeft);
                minTop = Math.Min(point.Y, minTop);
                maxLeft = Math.Max(point.X, maxLeft);
                maxTop = Math.Max(point.Y, maxTop);
            }

            Canvas.SetLeft(DesignerItem, Canvas.GetLeft(DesignerItem) + minLeft);
            Canvas.SetTop(DesignerItem, Canvas.GetTop(DesignerItem) + minTop);

            foreach (var thumb in thumbs)
            {
                Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) - minLeft);
                Canvas.SetTop(thumb, Canvas.GetTop(thumb) - minTop);
            }

            var points = new PointCollection();
            foreach (var point in Polyline.Points)
            {
                points.Add(new Point(point.X - minLeft, point.Y - minTop));
            }
            Polyline.Points = points;

            DesignerItem.ItemWidth = maxLeft - minLeft;
            DesignerItem.ItemHeight = maxTop - minTop;
        }

        public void UpdateZoom()
        {
            var zoom = DesignerCanvas.PlanDesignerViewModel.Zoom;
            foreach (var thumb in thumbs)
            {
                thumb.Width = 7 / zoom;
                thumb.Height = 7 / zoom;
                thumb.Margin = new Thickness(-3.5 / zoom, -3.5 / zoom, 0, 0);
            }
        }
    }
}
