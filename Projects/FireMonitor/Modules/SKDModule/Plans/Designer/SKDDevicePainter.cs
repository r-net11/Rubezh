using System.Windows.Controls;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Presenter;
using SKDModule.Events;
using SKDModule.ViewModels;

namespace SKDModule.Plans.Designer
{
	class SKDDevicePainter : BasePointPainter<SKDDevice, ShowSKDDeviceEvent>
	{
		public SKDDevicePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
		}

		protected override SKDDevice CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as ElementSKDDevice;
			return element == null ? null : PlanPresenter.Cache.Get<SKDDevice>(element.DeviceUID);
		}
		protected override StateTooltipViewModel<SKDDevice> CreateToolTip()
		{
			return new SKDDeviceTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);

			var contextMenu = new ContextMenu();
			contextMenu.Items.Add(Helper.CreateShowInTreeItem());
			contextMenu.Items.Add(UIHelper.BuildMenuItem("Показать связанные события", "pack://application:,,,/Controls;component/Images/BJournal.png", ShowJournalCommand));
			contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new DeviceDetailsViewModel(Item);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				Device = Item
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.SKDDevicePicture.GetDynamicBrush(Item);
		}
	}
}