using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Designer;
using FiresecAPI.Models;
using Infrastructure;
using Infrustructure.Plans.Events;
using PlansModule.Designer.DesignerItems;

namespace PlansModule.Designer
{
	public static class DesignerItemFactory
	{
		public static DesignerItem Create(ElementBase element)
		{
			var args = new DesignerItemFactoryEventArgs(element);
			ServiceFactory.Events.GetEvent<DesignerItemFactoryEvent>().Publish(args);
			if (args.DesignerItem != null)
				return args.DesignerItem;
			switch (element.Type)
			{
				case ElementType.Point:
					break;
				case ElementType.Rectangle:
					return new DesignerItemRectangle(element);
				case ElementType.Polygon:
					break;
				case ElementType.Polyline:
					break;
			}
			return new DesignerItemBase(element);
		}
	}
}
