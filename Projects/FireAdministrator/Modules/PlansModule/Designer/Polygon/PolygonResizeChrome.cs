using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlansModule.Designer
{
    public class PolygonResizeChrome : Canvas
    {
        public static PolygonResizeChrome Current { get; private set; }

        static PolygonResizeChrome()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PolygonResizeChrome), new FrameworkPropertyMetadata(typeof(PolygonResizeChrome)));
        }

        public PolygonResizeChrome(ContentControl designerItem)
        {
            Current = this;

            _designerItem = designerItem;
            _polygon = designerItem.Content as Polygon;

            Initialize();
        }

        public void Initialize()
        {
            ArrangeSize();

            Canvas canvas = this;
            canvas.Children.Clear();
            thumbs.Clear();

            foreach (var point in _polygon.Points)
            {
                Thumb thumb = new Thumb();
                thumb.Width = 10;
                thumb.Height = 10;
                thumb.Margin = new Thickness(-5, -5, 0, 0);
                Canvas.SetLeft(thumb, point.X + Canvas.GetLeft(_designerItem));
                Canvas.SetTop(thumb, point.Y + Canvas.GetTop(_designerItem));
                Canvas.SetLeft(thumb, point.X);
                Canvas.SetTop(thumb, point.Y);
                canvas.Children.Add(thumb);
                thumb.DragDelta += new DragDeltaEventHandler(_thumb_DragDelta);
                thumbs.Add(thumb);
            }
        }

        ContentControl _designerItem;
        Polygon _polygon;
        List<Thumb> thumbs = new List<Thumb>();

        private void _thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var currentThumb = sender as Thumb;

            Canvas.SetLeft(currentThumb, Canvas.GetLeft(currentThumb) + e.HorizontalChange);
            Canvas.SetTop(currentThumb, Canvas.GetTop(currentThumb) + e.VerticalChange);

            ArrangeSize();

            _polygon.Points.Clear();
            foreach (var thumb in thumbs)
            {
                _polygon.Points.Add(new Point(Canvas.GetLeft(thumb), Canvas.GetTop(thumb)));
            }
        }

        public void ArrangeSize()
        {
            double minLeft = double.MaxValue;
            double minTop = double.MaxValue;
            double maxLeft = 0;
            double maxTop = 0;
            foreach (var point in _polygon.Points)
            {
                minLeft = Math.Min(point.X, minLeft);
                minTop = Math.Min(point.Y, minTop);
                maxLeft = Math.Max(point.X, maxLeft);
                maxTop = Math.Max(point.Y, maxTop);
            }

            Trace.WriteLine(minLeft);
            Trace.WriteLine(minTop);
            Trace.WriteLine(maxLeft);
            Trace.WriteLine(maxTop);

            Canvas.SetLeft(_designerItem, Canvas.GetLeft(_designerItem) + minLeft);
            Canvas.SetTop(_designerItem, Canvas.GetTop(_designerItem) + minTop);

            foreach (var thumb in thumbs)
            {
                Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) - minLeft);
                Canvas.SetTop(thumb, Canvas.GetTop(thumb) - minTop);
            }

            var points = new PointCollection();
            foreach (var point in _polygon.Points)
            {
                points.Add(new Point(point.X - minLeft, point.Y - minTop));
            }
            _polygon.Points = points;

            _designerItem.Width = maxLeft - minLeft;
            _designerItem.Height = maxTop - minTop;
        }
    }
}
