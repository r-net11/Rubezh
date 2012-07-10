using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Controls;
using PlansModule.Designer.Adorners;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemRectangle : DesignerItemBase
	{
		public DesignerItemRectangle(ElementBase element)
			:base (element)
		{
			ResizeChrome = new ResizeChromeRectangle(this);
		}
	}
}
