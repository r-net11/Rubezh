using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Controls;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using Infrastructure.Client.Plans.ViewModels;
using SKDModule.Views;
using SKDModule.ViewModels;
using Infrastructure.Common.Windows;

namespace SKDModule.Plans.Designer
{
	class SKDDevicePainter : PointPainter
	{
		private PresenterItem _presenterItem;
		private SKDDevice Device;
		private ContextMenu _contextMenu;
		private DeviceTooltipViewModel _tooltip;

		public SKDDevicePainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			_contextMenu = null;
			var elementSKDDevice = presenterItem.Element as ElementSKDDevice;
			if (elementSKDDevice != null)
			{
				Device = Helper.GetSKDDevice(elementSKDDevice);
				if (Device != null && Device.State != null)
					Device.State.StateChanged += OnPropertyChanged;
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
			if (Device == null)
				return;

			if (_tooltip == null)
			{
				_tooltip = new DeviceTooltipViewModel(Device);
			}
			_tooltip.OnStateChanged();
		}

		public override object GetToolTip(string title)
		{
			return _tooltip;
		}
		protected override Brush GetBrush()
		{
			return PictureCacheSource.SKDDevicePicture.GetDynamicSKDBrush(Device);
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		private void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceEvent>().Publish(Device.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		private void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowSKDArchiveEventArgs()
			{
				Device = Device
			};
			ServiceFactory.Events.GetEvent<ShowSKDArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			DialogService.ShowWindow(new DeviceDetailsViewModel(Device));
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree);
				ShowJournalCommand = new RelayCommand(OnShowJournal);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Показать в дереве", 
					"pack://application:,,,/Controls;component/Images/BTree.png", 
					ShowInTreeCommand
				));
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Показать связанные события", 
					"pack://application:,,,/Controls;component/Images/BJournal.png", 
					ShowJournalCommand
				));
				_contextMenu.Items.Add(Helper.BuildMenuItem(
					"Свойства", 
					"pack://application:,,,/Controls;component/Images/BSettings.png", 
					ShowPropertiesCommand
				));
			}
			return _contextMenu;
		}
	}
}