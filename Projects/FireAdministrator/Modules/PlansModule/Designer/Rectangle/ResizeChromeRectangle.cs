using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PlansModule.Designer
{
    public class ResizeChromeRectangle : Control, IResizeChromeBase
    {
        static ResizeChromeRectangle()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChromeRectangle), new FrameworkPropertyMetadata(typeof(ResizeChromeRectangle)));
        }

        List<Ellipse> ellipses;
        List<ResizeThumbRectangle> resizeThumbs;
        Rectangle PART_CornerRectangle;
        Grid PART_ResizeGrid;
        Grid PART_Decorators;
        ResizeThumbRectangle PART_TopThumb;
        ResizeThumbRectangle PART_LeftThumb;
        ResizeThumbRectangle PART_RightThumb;
        ResizeThumbRectangle PART_BottomThumb;

        DesignerItem DesignerItem;

        DesignerCanvas DesignerCanvas
        {
            get { return DesignerItem.DesignerCanvas; }
        }

        public ResizeChromeRectangle(DesignerItem designerItem)
        {
            DesignerItem = designerItem;
            designerItem.ResizeChromeBase = this;
            this.Loaded += new RoutedEventHandler(ResizeChrome_Loaded);
        }

        public void Initialize()
        {
        }

        void ResizeChrome_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeResizeThumbs();
            UpdateZoom();
        }

        void InitializeResizeThumbs()
        {
            ellipses = new List<Ellipse>();
            resizeThumbs = new List<ResizeThumbRectangle>();

            PART_CornerRectangle = this.Template.FindName("PART_CornerRectangle", this) as Rectangle;
            PART_ResizeGrid = this.Template.FindName("PART_ResizeGrid", this) as Grid;
            PART_Decorators = this.Template.FindName("PART_Decorators", this) as Grid;

            PART_TopThumb = this.Template.FindName("PART_TopThumb", this) as ResizeThumbRectangle;
            PART_LeftThumb = this.Template.FindName("PART_LeftThumb", this) as ResizeThumbRectangle;
            PART_RightThumb = this.Template.FindName("PART_RightThumb", this) as ResizeThumbRectangle;
            PART_BottomThumb = this.Template.FindName("PART_BottomThumb", this) as ResizeThumbRectangle;

            resizeThumbs.Add(this.Template.FindName("PART_TopLeftThumb", this) as ResizeThumbRectangle);
            resizeThumbs.Add(this.Template.FindName("PART_TopRightThumb", this) as ResizeThumbRectangle);
            resizeThumbs.Add(this.Template.FindName("PART_BottomLeftThumb", this) as ResizeThumbRectangle);
            resizeThumbs.Add(this.Template.FindName("PART_BottomRightThumb", this) as ResizeThumbRectangle);

            ellipses.Add(this.Template.FindName("PART_TopLeftEllipse", this) as Ellipse);
            ellipses.Add(this.Template.FindName("PART_TopRightEllipse", this) as Ellipse);
            ellipses.Add(this.Template.FindName("PART_BottomLeftEllipse", this) as Ellipse);
            ellipses.Add(this.Template.FindName("PART_BottomRightEllipse", this) as Ellipse);

			//if (DesignerItem.IsDevice == false)
			//{
			//    foreach (var resizeThumb in resizeThumbs)
			//    {
			//        resizeThumb.InitializeDragEvents();
			//    }
			//    PART_TopThumb.InitializeDragEvents();
			//    PART_LeftThumb.InitializeDragEvents();
			//    PART_RightThumb.InitializeDragEvents();
			//    PART_BottomThumb.InitializeDragEvents();
			//}
        }

        public void UpdateZoom()
        {
            var zoom = DesignerCanvas.PlanDesignerViewModel.Zoom;

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
                //resizeThumb.IsEnabled = !DesignerItem.IsDevice;
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