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
			var element = presenterItem.Element as ElementXDevice;
			return element == null ? null : PlanPresenter.Cache.Get<XDevice>(element.XDeviceUID);
		}
		protected override StateTooltipViewModel<XDevice> CreateToolTip()
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
			return PictureCacheSource.XDevicePicture.GetDynamicBrush(Item);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showArchiveEventArgs = new ShowArchiveEventArgs()
			{
				GKDevice = Item
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showArchiveEventArgs);
		}
	}
}