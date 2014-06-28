using System;
using System.Windows.Controls;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Presenter;

namespace GKModule.Plans.Designer
{
	class XDevicePainter : BasePointPainter<XDevice, ShowXDeviceEvent>
	{
		public XDevicePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
		}

		protected override XDevice CreateItem(PresenterItem presenterItem)
		{
			var elementXDevice = presenterItem.Element as ElementXDevice;
			return elementXDevice == null ? null : PlanPresenter.Cache.Get<XDevice>(elementXDevice.XDeviceUID);
		}
		protected override StateTooltipViewModel<XDevice> CreateToolTip()
		{
			return new DeviceTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);

			var contextMenu = new ContextMenu();
			contextMenu.Items.Add(UIHelper.BuildMenuItem(
				"Показать в дереве",
				"pack://application:,,,/Controls;component/Images/BTree.png",
				ShowInTreeCommand
			));
			contextMenu.Items.Add(UIHelper.BuildMenuItem(
				"Показать связанные события",
				"pack://application:,,,/Controls;component/Images/BJournal.png",
				ShowJournalCommand
			));
			contextMenu.Items.Add(UIHelper.BuildMenuItem(
				"Свойства",
				"pack://application:,,,/Controls;component/Images/BSettings.png",
				ShowPropertiesCommand
			));
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new DeviceDetailsViewModel(Item);
		}
		protected override Guid ItemUID
		{
			get { return Item.BaseUID; }
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.XDevicePicture.GetDynamicBrush(Item);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Device = Item
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}
	}
}