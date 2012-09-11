using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace PlansModule.ViewModels
{
	public class ElementDeviceViewModel : BaseViewModel
	{
		public ElementDeviceView ElementDeviceView { get; private set; }
		public Guid DeviceUID { get; private set; }
		public DeviceState DeviceState { get; private set; }
		private ElementDevice _elementDevice;
		private Device _device;

		public ElementDeviceViewModel(ElementDevice elementDevice)
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree);
			DisableCommand = new RelayCommand(OnDisable, CanDisable);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			_elementDevice = elementDevice;
			DeviceUID = elementDevice.DeviceUID;
			_device = FiresecManager.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
			DeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
			if (DeviceState != null)
			{
				DeviceState.StateChanged += new Action(OnDeviceStateChanged);
			}
		}

		public Point Location
		{
			get { return new Point(_elementDevice.Left, _elementDevice.Top); }
		}

		public void DrawElementDevice()
		{
			if (ElementDeviceView != null)
				return;

			ElementDeviceView = new ElementDeviceView()
			{
				DataContext = this,
			};
			ElementDeviceView._deviceControl.IsManualUpdate = true;

			ElementDeviceView.Width = 10;
			ElementDeviceView.Height = 10;
			Canvas.SetLeft(ElementDeviceView, _elementDevice.Left - 5);
			Canvas.SetTop(ElementDeviceView, _elementDevice.Top - 5);

			if (_device != null)
			{
				ElementDeviceView._deviceControl.DriverId = _device.Driver.UID;
				OnDeviceStateChanged();
			}
		}

		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				DrawElementDevice();
				ElementDeviceView._selectationRectangle.StrokeThickness = value ? 1 : 0;
				OnPropertyChanged("IsSelected");

				if (value)
				{
					ElementDeviceView.Flush();
				}
			}
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.UID);
		}

		public bool CanDisable()
		{
			return FiresecManager.CanDisable(DeviceState);
		}

		public RelayCommand DisableCommand { get; private set; }
		void OnDisable()
		{
			if (ServiceFactory.SecurityService.Validate())
				FiresecManager.ChangeDisabled(DeviceState);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(_device.UID);
		}

		void OnDeviceStateChanged()
		{
			OnPropertyChanged("DeviceState");
			OnPropertyChanged("ToolTip");
			ElementDeviceView._deviceControl.StateType = DeviceState.StateType;
			ElementDeviceView._deviceControl.AdditionalStateCodes = new List<string>(
				from state in DeviceState.States
				select state.DriverState.Code);
			ElementDeviceView._deviceControl.Update();
		}

		public string ToolTip
		{
			get
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.Append(_device.PresentationAddress);
				stringBuilder.Append(" - ");
				stringBuilder.Append(_device.Driver.ShortName);
				stringBuilder.Append("\n");

				if (DeviceState.ParentStringStates != null)
				{
					foreach (var parentState in DeviceState.ParentStringStates)
					{
						stringBuilder.AppendLine(parentState);
					}
				}

				foreach (var state in DeviceState.States)
				{
					stringBuilder.AppendLine(state.DriverState.Name);
				}

				if (DeviceState.Parameters != null)
				{
					var nullString = "<NULL>";
					foreach (var parameter in DeviceState.Parameters)
					{
						if (string.IsNullOrEmpty(parameter.Value) || parameter.Value == nullString)
							continue;
						if ((parameter.Name == "Config$SerialNum") || (parameter.Name == "Config$SoftVersion"))
							continue;

						stringBuilder.Append(parameter.Caption);
						stringBuilder.Append(" - ");
						stringBuilder.AppendLine(parameter.Value);
					}
				}

				return stringBuilder.ToString();
			}
		}
	}
}