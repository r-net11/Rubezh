using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Controls;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using GKModule.Plans.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;

namespace GKModule.Plans.Designer
{
	class XDevicePainter : PointPainter
	{
		private PresenterItem _presenterItem;
		private XDevice _device;
		private ContextMenu _contextMenu;
		private XDeviceTooltipViewModel _tooltip;

		public XDevicePainter(PresenterItem presenterItem)
			: base(presenterItem.Element)
		{
			_contextMenu = null;
			var elementXDevice = presenterItem.Element as ElementXDevice;
			if (elementXDevice != null)
			{
				_device = Helper.GetXDevice(elementXDevice);
				if (_device != null && _device.DeviceState != null)
					_device.DeviceState.StateChanged += OnPropertyChanged;
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
				_tooltip = new XDeviceTooltipViewModel();
				_tooltip.TitleViewModel.Title = string.Format("{0} - {1}", _device.PresentationAddressAndDriver, _device.Driver.ShortName).TrimEnd();
				_tooltip.TitleViewModel.ImageSource = _device.Driver.ImageSource;
			}
			_tooltip.StateViewModel.Title = _device.DeviceState.StateClass.ToDescription();
			_tooltip.StateViewModel.ImageSource = _device.DeviceState.StateClass.ToIconSource();
			_tooltip.Update();
		}

		public override object GetToolTip(string title)
		{
			return _tooltip;
		}
		protected override Brush GetBrush()
		{
			return DevicePictureCache.GetDynamicXBrush(_device);
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(_device.UID);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(_device.UID);
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				ShowInTreeCommand = new RelayCommand(OnShowInTree);
				ShowPropertiesCommand = new RelayCommand(OnShowProperties);

				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в дереве",
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Свойства",
					Command = ShowPropertiesCommand
				});
			}
			return _contextMenu;
		}
	}
}