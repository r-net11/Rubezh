using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		ElementDevice ElementDevice;
		public Guid DeviceUID { get; private set; }
		Device Device;
		public DeviceState DeviceState { get; private set; }

		public ElementDeviceViewModel(ElementDevice elementDevice)
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree);
			DisableCommand = new RelayCommand(OnDisable, CanDisable);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
			if (deviceState != null)
			{
				elementDevice.DeviceState = deviceState;
			}
			ElementDevice = elementDevice;
			DeviceUID = elementDevice.DeviceUID;
			Device = elementDevice.Device;
			DeviceState = elementDevice.DeviceState;
			if (DeviceState != null)
			{
				DeviceState.StateChanged += new Action(OnDeviceStateChanged);
			}
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
			Canvas.SetLeft(ElementDeviceView, ElementDevice.Left);
			Canvas.SetTop(ElementDeviceView, ElementDevice.Top);

			if (Device != null)
			{
				ElementDeviceView._deviceControl.DriverId = Device.Driver.UID;
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
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Device.UID);
		}

		public bool CanDisable()
		{
			return DeviceState.CanDisable();
		}

		public RelayCommand DisableCommand { get; private set; }
		void OnDisable()
		{
			if (ServiceFactory.SecurityService.Validate())
				DeviceState.ChangeDisabled();
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(Device.UID);
		}

		void OnDeviceStateChanged()
		{
			OnPropertyChanged("DeviceState");
			ElementDeviceView._deviceControl.StateType = DeviceState.StateType;
			ElementDeviceView._deviceControl.AdditionalStateCodes = new List<string>(
				from state in DeviceState.States
				select state.DriverState.Code);
			ElementDeviceView._deviceControl.Update();

			UpdateTooltip();
		}

		string _toolTip;
		public string ToolTip
		{
			get { return _toolTip; }
			set
			{
				_toolTip = value;
				OnPropertyChanged("ToolTip");
			}
		}

		void UpdateTooltip()
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(Device.PresentationAddress);
			stringBuilder.Append(" - ");
			stringBuilder.Append(Device.Driver.ShortName);
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
				foreach (var parameter in DeviceState.Parameters.Where(x => x.Visible && string.IsNullOrEmpty(x.Value) == false && x.Value != nullString))
				{
					stringBuilder.Append(parameter.Caption);
					stringBuilder.Append(" - ");
					stringBuilder.AppendLine(parameter.Value);
				}
			}

			ToolTip = stringBuilder.ToString();
		}
	}
}