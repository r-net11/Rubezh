using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using SKDModule.Events;
using SKDModule.ViewModels;
using Infrastructure.Client.Plans;
using Common;
using FiresecAPI.GK;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace SKDModule.Plans.Designer
{
	class DoorPainter : BasePointPainter<Door, ShowDoorEvent>
	{
		public DoorPainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
		}

		protected override Door CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as ElementDoor;
			return element == null ? null : PlanPresenter.Cache.Get<Door>(element.DoorUID);
		}
		protected override StateTooltipViewModel<Door> CreateToolTip()
		{
			return new DoorTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			contextMenu.Items.Add(Helper.CreateShowInTreeItem());
			contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			//return new DoorDetailsViewModel(Item);
			return null;
		}

		protected override Brush GetBrush()
		{
			var background = PainterCache.GetBrush(GetStateColor());
			return PictureCacheSource.DoorPicture.GetBrush(background);
		}

		private Color GetStateColor()
		{
			//switch (Item.DoorState)
			//{
			//    case XStateClass.Unknown:
			//    case XStateClass.DBMissmatch:
			//    case XStateClass.TechnologicalRegime:
			//    case XStateClass.ConnectionLost:
			//    case XStateClass.HasNoLicense:
			//        return Colors.DarkGray;
			//    case XStateClass.Fire1:
			//    case XStateClass.Fire2:
			//        return Colors.Red;
			//    case XStateClass.Attention:
			//        return Colors.Yellow;
			//    case XStateClass.Ignore:
			//        return Colors.Yellow;
			//    case XStateClass.Norm:
			//        return Colors.Green;
			//    default:
			//        return Colors.White;
			//}
			return Colors.Green;
		}
	}
}