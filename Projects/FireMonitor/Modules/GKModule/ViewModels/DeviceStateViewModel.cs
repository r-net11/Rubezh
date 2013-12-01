using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI;
using Controls.Converters;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class DeviceStateViewModel : BaseViewModel
	{
		public XDeviceState DeviceState { get; private set; }

		public DeviceStateViewModel(XDeviceState deviceState)
		{
			DeviceState = deviceState;
			StateClasses = new ObservableCollection<XStateClassViewModel>();
			AdditionalStates = new ObservableCollection<XAdditionalState>();
			DeviceState.StateChanged += new Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			if (DeviceState == null)
				return;

			try
			{
				OnPropertyChanged("DeviceState");
				OnPropertyChanged("StateClassName");

				StateClasses.Clear();
				foreach (var stateClass in DeviceState.StateClasses)
				{
					if (stateClass != DeviceState.StateClass)
					{
						StateClasses.Add(new XStateClassViewModel(DeviceState.Device, stateClass));
					}
				}

				AdditionalStates.Clear();
				foreach (var additionalState in DeviceState.AdditionalStates)
				{
					AdditionalStates.Add(additionalState);
				}
			}
			catch { }
		}

		public ObservableCollection<XStateClassViewModel> StateClasses { get; private set; }
		public ObservableCollection<XAdditionalState> AdditionalStates { get; private set; }

		public string StateClassName
		{
			get
			{
				var result = XStateClassViewModel.GetStateName(DeviceState.StateClass, DeviceState.Device);
				return result;
			}
		}
	}

	public class XStateClassViewModel : BaseViewModel
	{
		public XStateClass StateClass { get; private set; }
		XDevice Device { get; set; }

		public XStateClassViewModel(XDevice device, XStateClass stateClass)
		{
			Device = device;
			StateClass = stateClass;
		}

		public string StateClassName
		{
			get
			{
				var result = GetStateName(StateClass, Device);
				return result;
			}
		}

		public static string GetStateName(XStateClass stateClass, XDevice device)
		{
			if (device != null)
			{
				if (device.DriverType == XDriverType.Valve)
				{
					switch (stateClass)
					{
						case XStateClass.On:
							return "Открыто";

						case XStateClass.Off:
							return "Закрыто";

						case XStateClass.TurningOn:
							return "Открывается";

						case XStateClass.TurningOff:
							return "Закрывается";
					}
				}
				if (device.DriverType == XDriverType.AM1_T)
				{
					if (stateClass == XStateClass.Fire2)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "OnMessage");
						if (property != null)
						{
							if (!string.IsNullOrEmpty(property.StringValue))
								return property.StringValue;
						}
					}
					if (stateClass == XStateClass.Norm)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "NormMessage");
						if (property != null)
						{
							if (!string.IsNullOrEmpty(property.StringValue))
								return property.StringValue;
						}
					}
				}
			}
			if (stateClass == XStateClass.Fire1)
			{
				return "Сработка 1";
			}
			if (stateClass == XStateClass.Fire2)
			{
				return "Сработка 2";
			}
			return stateClass.ToDescription();
		}
	}
}