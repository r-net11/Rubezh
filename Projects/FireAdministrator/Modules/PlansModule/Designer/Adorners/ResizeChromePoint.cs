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
using Infrustructure.Plans.Elements;

namespace PlansModule.Designer.Adorners
{
	public class ResizeChromePoint : ResizeChrome
	{
		static ResizeChromePoint()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChromePoint), new FrameworkPropertyMetadata(typeof(ResizeChromePoint)));
		}

		public ResizeChromePoint(DesignerItem designerItem)
		{
			DesignerItem = designerItem;
			Loaded += (s, e) => UpdateZoom();
		}

		public override void Initialize()
		{
		}
		protected override void Resize(ResizeDirection direction, double horizontalChange, double verticalChange)
		{
			ElementBasePoint element = DesignerItem.Element as ElementBasePoint;
			if (element != null)
			{
				if ((direction & ResizeDirection.Top) == ResizeDirection.Top || (direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
					element.Top += verticalChange;
				if ((direction & ResizeDirection.Left) == ResizeDirection.Left || (direction & ResizeDirection.Right) == ResizeDirection.Right)
					element.Left += horizontalChange;
				DesignerItem.SetLocation();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
	}
}
