using Infrastructure.Common.Services;
using Infrastructure.Designer.DesignerItems;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

namespace Infrastructure.Designer
{
	public static class DesignerItemFactory
	{
		public static DesignerItem Create(ElementBase element)
		{
			var args = new DesignerItemFactoryEventArgs(element);
			ServiceFactoryBase.Events.GetEvent<DesignerItemFactoryEvent>().Publish(args);
			if (args.DesignerItem == null)
				switch (element.Type)
				{
					case ElementType.Point:
						args.DesignerItem = new DesignerItemPoint(element);
						break;
					case ElementType.Rectangle:
						args.DesignerItem = new DesignerItemRectangle(element);
						break;
					case ElementType.Polygon:
					case ElementType.Polyline:
						args.DesignerItem = new DesignerItemShape(element);
						break;
				}
			if (args.DesignerItem == null)
				args.DesignerItem = new DesignerItemBase(element);
			return args.DesignerItem;
		}
	}
}
