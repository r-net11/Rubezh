using Infrastructure;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using PlansModule.Designer.DesignerItems;
using System.Windows.Controls;
using Infrastructure.Common;
using PlansModule.Designer.Designer;
using Infrastructure.Events;

namespace PlansModule.Designer
{
	public static class DesignerItemFactory
	{
		public static DesignerItem Create(ElementBase element)
		{
			var args = new DesignerItemFactoryEventArgs(element);
			ServiceFactory.Events.GetEvent<DesignerItemFactoryEvent>().Publish(args);
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
			if (element is IElementZone)
				args.DesignerItem.ContextMenu.Items.Add(new MenuItem()
				{
					CommandParameter = args.DesignerItem,
					Command = new RelayCommand<DesignerItem>(OnEditZone, CanEditZone),
					Header = "Редактировать"
				});
			return args.DesignerItem;
		}

		private static void OnEditZone(DesignerItem designerItem)
		{
			var elementZone = designerItem.Element as IElementZone;
			//var zone = Helper.GetZone(elementZone);
			ServiceFactory.Events.GetEvent<EditZoneEvent>().Publish(elementZone.ZoneNo.Value);
			var color = elementZone.BackgroundColor;
			Helper.SetZone(elementZone);
			if (color != elementZone.BackgroundColor)
				designerItem.Redraw();
		}
		private static bool CanEditZone(DesignerItem designerItem)
		{
			return (designerItem.Element as IElementZone).ZoneNo.HasValue;
		}
	}
}
