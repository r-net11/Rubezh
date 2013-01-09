using System.Windows;
using Infrastructure;
using Infrustructure.Plans.Designer;
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
			: base(designerItem)
		{
		}

		public override void Initialize()
		{
		}
		protected override void Resize(ResizeDirection direction, Vector vector)
		{
			ElementBasePoint element = DesignerItem.Element as ElementBasePoint;
			if (element != null)
			{
				if ((direction & ResizeDirection.Top) == ResizeDirection.Top || (direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
					element.Top += vector.Y;
				if ((direction & ResizeDirection.Left) == ResizeDirection.Left || (direction & ResizeDirection.Right) == ResizeDirection.Right)
					element.Left += vector.X;
				DesignerItem.Redraw();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
	}
}
