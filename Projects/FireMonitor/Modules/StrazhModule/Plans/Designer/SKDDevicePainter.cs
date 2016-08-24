using System.Windows.Controls;
using System.Windows.Media;
using DeviceControls;
using Localization.Strazh.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Presenter;
using StrazhModule.ViewModels;

namespace StrazhModule.Plans.Designer
{
	class SKDDevicePainter : BasePointPainter<SKDDevice, ShowSKDDeviceEvent>
	{
		private DeviceViewModel _deviceViewModel;

		public SKDDevicePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_deviceViewModel = new ViewModels.DeviceViewModel(Item);
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

			if (_deviceViewModel.IsLock)
			{
				contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ResetBreakinState, null, _deviceViewModel.ClearPromptWarningCommand));
				contextMenu.Items.Add(new Separator());
				contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ModeOpen, null, _deviceViewModel.DeviceAccessStateOpenAlwaysCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ModeNorm, null, _deviceViewModel.DeviceAccessStateNormalCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ModeClose, null, _deviceViewModel.DeviceAccessStateCloseAlwaysCommand));
				contextMenu.Items.Add(new Separator());
				contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.Open, null, _deviceViewModel.OpenCommand));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.Close, null, _deviceViewModel.CloseCommand));
				contextMenu.Items.Add(new Separator());
			}
			
			contextMenu.Items.Add(Helper.CreateShowInTreeItem());
			contextMenu.Items.Add(UIHelper.BuildMenuItem(CommonResources.ShowRelatedEvents, "pack://application:,,,/Controls;component/Images/BJournal.png", ShowJournalCommand));
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
				SKDDevice = Item
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		protected override Brush GetBrush()
		{
			return PictureCacheSource.SKDDevicePicture.GetDynamicBrush(Item);
		}
	}
}