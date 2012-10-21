using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;
using FiresecAPI;
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
		public DeviceState DeviceState
		{
			get { return Device.DeviceState; }
		}

		public ElementDeviceViewModel(ElementDevice elementDevice)
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree);
			DisableCommand = new RelayCommand(OnDisable, CanDisable);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			ElementDevice = elementDevice;
			DeviceUID = elementDevice.DeviceUID;
			Device = FiresecManager.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
			if (Device != null)
			{
				DeviceState.StateChanged += new Action(OnDeviceStateChanged);
				DeviceState.ParametersChanged += new Action(OnParametersChanged);
			}
		}

		public Point Location
		{
			get { return new Point(ElementDevice.Left, ElementDevice.Top); }
		}

		public void DrawElementDevice()
		{
			if (ElementDeviceView != null)
				return;

			ElementDeviceView = new ElementDeviceView()
			{
				DataContext = this,
			};

			ElementDeviceView.Width = 10;
			ElementDeviceView.Height = 10;
			Canvas.SetLeft(ElementDeviceView, ElementDevice.Left - 5);
			Canvas.SetTop(ElementDeviceView, ElementDevice.Top - 5);

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
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(Device.UID);
		}

		void OnDeviceStateChanged()
		{
			OnPropertyChanged("DeviceState");
			OnPropertyChanged("ToolTip");
			ElementDeviceView._deviceControl.StateType = DeviceState.StateType;
			ElementDeviceView._deviceControl.AdditionalStateCodes = new List<string>(
                from state in DeviceState.ThreadSafeStates
				select state.DriverState.Code);
			ElementDeviceView._deviceControl.Update();
		}

		void OnParametersChanged()
		{
			OnPropertyChanged("ToolTip");
		}

		public string ToolTip
		{
			get
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Device.PresentationAddress + " - " + Device.Driver.ShortName);
				stringBuilder.AppendLine("Состояние: " + DeviceState.StateType.ToDescription());

				if (DeviceState.ParentStringStates != null)
				{
					foreach (var parentState in DeviceState.ParentStringStates)
					{
						stringBuilder.AppendLine(parentState);
					}
				}

                foreach (var state in DeviceState.ThreadSafeStates)
				{
					stringBuilder.AppendLine(state.DriverState.Name);
				}

				if (DeviceState.Parameters != null)
				{
					foreach (var parameter in DeviceState.Parameters)
					{
						if (string.IsNullOrEmpty(parameter.Value) || parameter.Value == "<NULL>")
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