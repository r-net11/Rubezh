using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Designer;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using Infrastructure;

namespace PlansModule.Designer.Adorners
{
	public class RectangleResizeChrome : ResizeChrome
	{
		static RectangleResizeChrome()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(RectangleResizeChrome), new FrameworkPropertyMetadata(typeof(RectangleResizeChrome)));
		}

		public RectangleResizeChrome(DesignerItem designerItem)
		{
			DesignerItem = designerItem;
			Loaded += (s, e) => UpdateZoom();
			AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(ResizeThumb_DragDelta));
		}

		public override void Initialize()
		{
		}
		public void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (DesignerItem.IsSelected)
			{
				ResizeThumb thumb = (ResizeThumb)e.OriginalSource;
				double minLeft = double.MaxValue;
				double minTop = double.MaxValue;
				double minDeltaHorizontal = double.MaxValue;
				double minDeltaVertical = double.MaxValue;
				double dragDeltaVertical, dragDeltaHorizontal;

				foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
				{
					minLeft = Math.Min(Canvas.GetLeft(designerItem), minLeft);
					minTop = Math.Min(Canvas.GetTop(designerItem), minTop);

					minDeltaVertical = Math.Min(minDeltaVertical, designerItem.ActualHeight - designerItem.MinHeight);
					minDeltaHorizontal = Math.Min(minDeltaHorizontal, designerItem.ActualWidth - designerItem.MinWidth);
				}

				foreach (var designerItem in DesignerCanvas.SelectedItems)
				{
					switch (thumb.VerticalAlignment)
					{
						case VerticalAlignment.Bottom:
							dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
							designerItem.Height = designerItem.ActualHeight - dragDeltaVertical;
							break;
						case VerticalAlignment.Top:
							dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
							Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + dragDeltaVertical);
							designerItem.Height = designerItem.ActualHeight - dragDeltaVertical;
							break;
					}

					switch (thumb.HorizontalAlignment)
					{
						case HorizontalAlignment.Left:
							dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
							Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + dragDeltaHorizontal);
							designerItem.Width = designerItem.ActualWidth - dragDeltaHorizontal;
							break;
						case HorizontalAlignment.Right:
							dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
							designerItem.Width = designerItem.ActualWidth - dragDeltaHorizontal;
							break;
					}
				}

				e.Handled = true;
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
	}
}
