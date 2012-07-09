using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Designer;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;

namespace PlansModule.Designer.Adorners
{
	public class RectangleResizeChrome : ResizeChrome
	{
		static RectangleResizeChrome()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(RectangleResizeChrome), new FrameworkPropertyMetadata(typeof(RectangleResizeChrome)));
		}

		List<Ellipse> ellipses;
		List<ResizeThumb> resizeThumbs;
		Rectangle PART_CornerRectangle;
		Grid PART_ResizeGrid;
		Grid PART_Decorators;
		ResizeThumb PART_TopThumb;
		ResizeThumb PART_LeftThumb;
		ResizeThumb PART_RightThumb;
		ResizeThumb PART_BottomThumb;

		DesignerItem DesignerItem;

		public CommonDesignerCanvas DesignerCanvas
		{
			get { return DesignerItem.DesignerCanvas; }
		}

		public RectangleResizeChrome(DesignerItem designerItem)
		{
			DesignerItem = designerItem;
			Loaded += new RoutedEventHandler(ResizeChrome_Loaded);
		}

		private void ResizeChrome_Loaded(object sender, RoutedEventArgs e)
		{
			InitializeResizeThumbs();
			UpdateZoom();
		}

		void InitializeResizeThumbs()
		{
			ellipses = new List<Ellipse>();
			resizeThumbs = new List<ResizeThumb>();

			PART_CornerRectangle = this.Template.FindName("PART_CornerRectangle", this) as Rectangle;
			PART_ResizeGrid = this.Template.FindName("PART_ResizeGrid", this) as Grid;
			PART_Decorators = this.Template.FindName("PART_Decorators", this) as Grid;

			PART_TopThumb = this.Template.FindName("PART_TopThumb", this) as ResizeThumb;
			PART_LeftThumb = this.Template.FindName("PART_LeftThumb", this) as ResizeThumb;
			PART_RightThumb = this.Template.FindName("PART_RightThumb", this) as ResizeThumb;
			PART_BottomThumb = this.Template.FindName("PART_BottomThumb", this) as ResizeThumb;

			resizeThumbs.Add(this.Template.FindName("PART_TopLeftThumb", this) as ResizeThumb);
			resizeThumbs.Add(this.Template.FindName("PART_TopRightThumb", this) as ResizeThumb);
			resizeThumbs.Add(this.Template.FindName("PART_BottomLeftThumb", this) as ResizeThumb);
			resizeThumbs.Add(this.Template.FindName("PART_BottomRightThumb", this) as ResizeThumb);

			//ellipses.Add(this.Template.FindName("PART_TopLeftEllipse", this) as Ellipse);
			//ellipses.Add(this.Template.FindName("PART_TopRightEllipse", this) as Ellipse);
			//ellipses.Add(this.Template.FindName("PART_BottomLeftEllipse", this) as Ellipse);
			//ellipses.Add(this.Template.FindName("PART_BottomRightEllipse", this) as Ellipse);
		}

		public override void Initialize()
		{
		}
		public override void UpdateZoom()
		{
			//if (!IsInitialized)
			//    return;
			var zoom = DesignerCanvas.Zoom;
			//PART_ResizeGrid.Margin = new Thickness(-3 / zoom);
			//PART_Decorators.Margin = new Thickness(-3 / zoom);
			//PART_CornerRectangle.Margin = new Thickness(1 / zoom);
			//PART_CornerRectangle.StrokeThickness = 1 / zoom;

			//PART_TopThumb.Height = 3 / zoom;
			//PART_LeftThumb.Width = 3 / zoom;
			//PART_RightThumb.Width = 3 / zoom;
			//PART_BottomThumb.Height = 3 / zoom;

			//foreach (var resizeThumb in resizeThumbs)
			//{
			//    resizeThumb.Width = 7 / zoom;
			//    resizeThumb.Height = 7 / zoom;
			//    //resizeThumb.IsEnabled = !DesignerItem.IsDevice;
			//}

			//foreach (var ellipse in ellipses)
			//{
			//    ellipse.Width = 7 / zoom;
			//    ellipse.Height = 7 / zoom;
			//    ellipse.StrokeThickness = 0.5 / zoom;
			//    ellipse.Margin = new Thickness(-2 / zoom);
			//}
		}
	}
}
