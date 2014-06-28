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

namespace SKDModule.Plans.Designer
{
	class SKDDevicePainter : PointPainter
	{
		private PresenterItem _presenterItem;
		private SKDDevice _device;
		private ContextMenu _contextMenu;
		private SKDDeviceTooltipViewModel _tooltip;

		public SKDDevicePainter(PresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			_contextMenu = null;
			var elementSKDDevice = presenterItem.Element as ElementSKDDevice;
			if (elementSKDDevice != null)
			{
				_device = PlanPresenter.Cache.Get<SKDDevice>(elementSKDDevice.DeviceUID);
				if (_device != null && _device.State != null)
					_device.State.StateChanged += OnPropertyChanged;
			}
			_presenterItem = presenterItem;
			_presenterItem.IsPoint = true;
			_presenterItem.ShowBorderOnMouseOver = true;
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			_presenterItem.Cursor = Cursors.Hand;
			_presenterItem.ClickEvent += (s, e) => OnShowProperties();
			UpdateTooltip();
		}

		private void OnPropertyChanged()
		{
			if (_presenterItem != null)
			{
				UpdateTooltip();
				_presenterItem.InvalidatePainter();
				_presenterItem.DesignerCanvas.Refresh();
			}
		}
		private void UpdateTooltip()
		{
			if (_device == null)
				return;

			if (_tooltip == null)
			{
				_tooltip = new SKDDeviceTooltipViewModel(_device);
			}
			_tooltip.OnStateChanged();
		}

		public override object GetToolTip(string title)
		{
			return _tooltip;
		}
		protected override Brush GetBrush()
		{
			return PictureCacheSource.SKDDevicePicture.GetDynamicBrush(_device);
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		private void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceEvent>().Publish(_device.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowSKDArchiveEventArgs()
			{
				Device = _device
			};
			ServiceFactory.Events.GetEvent<ShowSKDArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(_device));
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree);
				ShowJournalCommand = new RelayCommand(OnShowJournal);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать в дереве", 
					"pack://application:,,,/Controls;component/Images/BTree.png", 
					ShowInTreeCommand
				));
				_contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать связанные события", 
					"pack://application:,,,/Controls;component/Images/BJournal.png", 
					ShowJournalCommand
				));
				_contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Свойства", 
					"pack://application:,,,/Controls;component/Images/BSettings.png", 
					ShowPropertiesCommand
				));
			}
			return _contextMenu;
		}
	}
}