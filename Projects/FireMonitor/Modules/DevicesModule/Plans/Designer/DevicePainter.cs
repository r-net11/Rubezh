using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Painters;
using System.Windows;
using Infrustructure.Plans.Elements;
using FiresecClient;
using FiresecAPI.Models;
using DeviceControls;
using FiresecAPI;
using Infrustructure.Plans.Presenter;
using System.Windows.Controls;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Shapes;
using System.Windows.Media;

namespace DevicesModule.Plans.Designer
{
	class DevicePainter : BaseViewModel, IPainter
	{
		private PresenterItem _presenterItem;
		private DeviceControl _deviceControl;
		private Device _device;

		public DevicePainter()
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree);
			DisableCommand = new RelayCommand(OnDisable, CanDisable);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			_deviceControl = new DeviceControl();
		}

		public void Bind(PresenterItem presenterItem)
		{
			_presenterItem = presenterItem;
			_presenterItem.IsPoint = true;
			_presenterItem.Border = BorderHelper.CreateBorderRectangle();
			_presenterItem.ContextMenu = (ContextMenu)_presenterItem.FindResource("DeviceMenuView");
			_presenterItem.ContextMenu.DataContext = this;
			var elementDevice = presenterItem.Element as ElementDevice;
			if (elementDevice != null)
			{
				_device = FiresecManager.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
				if (_device != null)
				{
					_deviceControl.DriverId = _device.Driver.UID;
					_deviceControl.StateType = _device.DeviceState.StateType;
					_deviceControl.AdditionalStateCodes = _device.DeviceState.ThreadSafeStates.ConvertAll(item => item.DriverState.Code);
					_device.DeviceState.StateChanged += OnPropertyChanged;
					_device.DeviceState.ParametersChanged += () => presenterItem.Title = GetDeviceTooltip();
				}
			}
			_presenterItem.Title = GetDeviceTooltip();
			OnPropertyChanged(() => DeviceState);
		}

		private void OnPropertyChanged()
		{
			_deviceControl.StateType = _device.DeviceState.StateType;
			_deviceControl.AdditionalStateCodes = _device.DeviceState.ThreadSafeStates.ConvertAll(item => item.DriverState.Code);
			_presenterItem.Title = GetDeviceTooltip();
			_presenterItem.Redraw();
			OnPropertyChanged(() => DeviceState);
		}
		private string GetDeviceTooltip()
		{
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

		#region IPainter Members

		public FrameworkElement Draw(ElementBase element)
		{
			_deviceControl.Update();
			return _deviceControl;
		}

		#endregion

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

	}
}
