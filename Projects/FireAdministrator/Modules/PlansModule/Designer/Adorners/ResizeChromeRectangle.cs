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
	public class ResizeChromeRectangle : ResizeChrome
	{
		static ResizeChromeRectangle()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChromeRectangle), new FrameworkPropertyMetadata(typeof(ResizeChromeRectangle)));
		}

		public ResizeChromeRectangle(DesignerItem designerItem)
			: base(designerItem)
		{
		}

		public override void Initialize()
		{
		}

		protected override void Resize(ResizeDirection direction, Vector vector)
		{
			ElementBaseRectangle element = DesignerItem.Element as ElementBaseRectangle;
			if (element != null)
			{
				if ((direction & ResizeDirection.Top) == ResizeDirection.Top)
				{
					element.Top += vector.Y;
					element.Height -= vector.Y;
				}
				else if ((direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
					element.Height += vector.Y;
				if ((direction & ResizeDirection.Left) == ResizeDirection.Left)
				{
					element.Left += vector.X;
					element.Width -= vector.X;
				}
				else if ((direction & ResizeDirection.Right) == ResizeDirection.Right)
					element.Width += vector.X;
				DesignerItem.SetLocation();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
	}
}
