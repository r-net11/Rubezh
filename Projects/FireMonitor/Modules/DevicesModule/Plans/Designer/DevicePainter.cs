using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using System.Windows.Controls;
using Controls.Converters;

namespace DevicesModule.Plans.Designer
{
	class DevicePainter : PointPainter
	{
		private PresenterItem _presenterItem;
		private DeviceControl _deviceControl;
		private Device _device;
		private ContextMenu _contextMenu;

		public DevicePainter(ElementDevice elementDevice)
			: base(elementDevice)
		{
			_contextMenu = null;
			ShowInTreeCommand = new RelayCommand(OnShowInTree);
			DisableCommand = new RelayCommand(OnDisable, CanDisable);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		public void Bind(PresenterItem presenterItem)
		{
			_presenterItem = presenterItem;
			_presenterItem.IsPoint = true;
			_presenterItem.SetBorder(new PresenterBorder(_presenterItem));
			_presenterItem.ContextMenuProvider = CreateContextMenu;
			var elementDevice = presenterItem.Element as ElementDevice;
			if (elementDevice != null)
			{
				_device = FiresecManager.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
				if (_device != null)
				{
					_deviceControl = new DeviceControl();
					_deviceControl.DriverId = _device.Driver.UID;
					_deviceControl.StateType = _device.DeviceState.StateType;
					_deviceControl.AdditionalStateCodes = _device.DeviceState.ThreadSafeStates.ConvertAll(item => item.DriverState.Code);
					_device.DeviceState.StateChanged += OnPropertyChanged;
					_device.DeviceState.ParametersChanged += () => presenterItem.Title = GetDeviceTooltip();
				}
			}
			_presenterItem.Title = GetDeviceTooltip();
		}

		private void OnPropertyChanged()
		{
			_deviceControl.StateType = _device.DeviceState.StateType;
			_deviceControl.AdditionalStateCodes = _device.DeviceState.ThreadSafeStates.ConvertAll(item => item.DriverState.Code);
			_presenterItem.Title = GetDeviceTooltip();
			_presenterItem.Redraw();
		}
		private string GetDeviceTooltip()
		{
			if (_device == null)
				return null;
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(_device.PresentationAddressAndName);
			stringBuilder.AppendLine("Состояние: " + _device.DeviceState.StateType.ToDescription());

			if (_device.DeviceState.ParentStringStates != null)
				foreach (var parentState in _device.DeviceState.ParentStringStates)
					stringBuilder.AppendLine(parentState);

			foreach (var state in _device.DeviceState.ThreadSafeStates)
				stringBuilder.AppendLine(state.DriverState.Name);

			if (_device.DeviceState.ThreadSafeParameters != null)
				foreach (var parameter in _device.DeviceState.ThreadSafeParameters)
				{
					if (string.IsNullOrEmpty(parameter.Value) || parameter.Value == "<NULL>")
						continue;
					if ((parameter.Name == "Config$SerialNum") || (parameter.Name == "Config$SoftVersion"))
						continue;

					stringBuilder.Append(parameter.Caption);
					stringBuilder.Append(" - ");
					stringBuilder.AppendLine(parameter.Value);
				}
			return stringBuilder.ToString().TrimEnd();
		}

		protected override Brush GetBrush()
		{
			return DevicePictureCache.GetBrush(_device);
		}

		public DeviceState DeviceState
		{
			get { return _device == null ? null : _device.DeviceState; }
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.UID);
		}
		public bool CanDisable()
		{
			return _device == null ? false : FiresecManager.CanDisable(_device.DeviceState);
		}
		public RelayCommand DisableCommand { get; private set; }
		void OnDisable()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.ChangeDisabled(_device.DeviceState);
		}
		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(_device.UID);
		}

		private ContextMenu CreateContextMenu()
		{
			if (_contextMenu == null)
			{
				_contextMenu = new ContextMenu();
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Показать в дереве",
					Command = ShowInTreeCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "",
					Command = DisableCommand
				});
				_contextMenu.Items.Add(new MenuItem()
				{
					Header = "Свойства",
					Command = ShowPropertiesCommand
				});
			}
			((MenuItem)_contextMenu.Items[1]).Header = DeviceState.IsDisabled ? "Включить" : "Отключить";
			return _contextMenu;
		}
	}
}