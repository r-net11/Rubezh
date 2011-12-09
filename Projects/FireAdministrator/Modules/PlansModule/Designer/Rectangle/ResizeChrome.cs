using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PlansModule.Designer
{
    public class ResizeChrome : Control
    {
        static ResizeChrome()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChrome), new FrameworkPropertyMetadata(typeof(ResizeChrome)));
        }

        DesignerItem DesignerItem;

        DesignerCanvas DesignerCanvas
        {
            get { return DesignerItem.DesignerCanvas; }
        }

        double ZoomFactor
        {
            get { return DesignerCanvas.PlanDesignerViewModel.Zoom; }
        }

        public ResizeChrome(DesignerItem designerItem)
        {
            DesignerItem = designerItem;
            designerItem.ResizeChrome = this;
            this.Loaded += new RoutedEventHandler(ResizeChrome_Loaded);
        }

        void ResizeChrome_Loaded(object sender, RoutedEventArgs e)
        {
            Zoom(ZoomFactor);
        }

        public void Zoom(double zoom)
        {
            List<Ellipse> ellipses = new List<Ellipse>();
            List<ResizeThumb> resizeThumbs = new List<ResizeThumb>();

            var PART_CornerRectangle = this.Template.FindName("PART_CornerRectangle", this) as Rectangle;
            var PART_ResizeGrid = this.Template.FindName("PART_ResizeGrid", this) as Grid;
            var PART_Decorators = this.Template.FindName("PART_Decorators", this) as Grid;

            var PART_TopThumb = this.Template.FindName("PART_TopThumb", this) as ResizeThumb;
            var PART_LeftThumb = this.Template.FindName("PART_LeftThumb", this) as ResizeThumb;
            var PART_RightThumb = this.Template.FindName("PART_RightThumb", this) as ResizeThumb;
            var PART_BottomThumb = this.Template.FindName("PART_BottomThumb", this) as ResizeThumb;

            resizeThumbs.Add(this.Template.FindName("PART_TopLeftThumb", this) as ResizeThumb);
            resizeThumbs.Add(this.Template.FindName("PART_TopRightThumb", this) as ResizeThumb);
            resizeThumbs.Add(this.Template.FindName("PART_BottomLeftThumb", this) as ResizeThumb);
            resizeThumbs.Add(this.Template.FindName("PART_BottomRightThumb", this) as ResizeThumb);

            ellipses.Add(this.Template.FindName("PART_TopLeftEllipse", this) as Ellipse);
            ellipses.Add(this.Template.FindName("PART_TopRightEllipse", this) as Ellipse);
            ellipses.Add(this.Template.FindName("PART_BottomLeftEllipse", this) as Ellipse);
            ellipses.Add(this.Template.FindName("PART_BottomRightEllipse", this) as Ellipse);


            PART_ResizeGrid.Margin = new Thickness(-3 / zoom);
            PART_Decorators.Margin = new Thickness(-3 / zoom);
            PART_CornerRectangle.Margin = new Thickness(1 / zoom);
            PART_CornerRectangle.StrokeThickness = 1 / zoom;

            PART_TopThumb.Height = 3 / zoom;
            PART_LeftThumb.Width = 3 / zoom;
            PART_RightThumb.Width = 3 / zoom;
            PART_BottomThumb.Height = 3 / zoom;

            foreach (var resizeThumb in resizeThumbs)
            {
                resizeThumb.Width = 7 / zoom;
                resizeThumb.Height = 7 / zoom;
            }

            foreach (var ellipse in ellipses)
            {
                ellipse.Width = 7 / zoom;
                ellipse.Height = 7 / zoom;
                ellipse.StrokeThickness = 0.5 / zoom;
                ellipse.Margin = new Thickness(-2 / zoom);
            }
        }
    }
}