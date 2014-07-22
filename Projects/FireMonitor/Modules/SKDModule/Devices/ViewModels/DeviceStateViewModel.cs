using System;
using System.Collections.ObjectModel;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DeviceStateViewModel : BaseViewModel
	{
		public SKDDeviceState State { get; private set; }

		public DeviceStateViewModel(SKDDeviceState deviceState)
		{
			State = deviceState;
			StateClasses = new ObservableCollection<XStateClassViewModel>();
			State.StateChanged += new Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");

			StateClasses.Clear();
			foreach (var stateClass in State.StateClasses)
			{
				StateClasses.Add(new XStateClassViewModel(State.Device, stateClass));
			}
		}

		public ObservableCollection<XStateClassViewModel> StateClasses { get; private set; }
	}

	public class XStateClassViewModel : BaseViewModel
	{
		public XStateClass StateClass { get; private set; }
		SKDDevice Device { get; set; }

		public XStateClassViewModel(SKDDevice device, XStateClass stateClass)
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

		public static string GetStateName(XStateClass stateClass, SKDDevice device)
		{
			if (device.DriverType == SKDDriverType.Lock)
			{
				switch(stateClass)
				{
					case XStateClass.Off:
						return "Закрыто";

					case XStateClass.On:
						return "Открыто";
				}
				return stateClass.ToDescription();
			}

			return stateClass.ToDescription();
		}
	}
}