using DeviceControls;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Plans;
using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace GKModule.Plans.Designer
{
	class GKDevicePainter : BasePointPainter<GKDevice, ShowGKDeviceEvent>
	{
		public GKDevicePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
		}

		protected override GKDevice CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as ElementGKDevice;
			return element == null ? null : PlanPresenter.Cache.Get<GKDevice>(element.DeviceUID);
		}
		protected override StateTooltipViewModel<GKDevice> CreateToolTip()
		{
			return new DeviceTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);

			var contextMenu = new ContextMenu();
			contextMenu.Items.Add(Helper.CreateShowInTreeItem());
			contextMenu.Items.Add(UIHelper.BuildMenuItem(
				"Показать связанные события",
				"pack://application:,,,/Controls;component/Images/BJournal.png",
				ShowJournalCommand
			));
			contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new DeviceDetailsViewModel(Item);
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.GKDevicePicture.GetDynamicBrush(Item);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			if (Item != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Item.UID });
		}
	}
}