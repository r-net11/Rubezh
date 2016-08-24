using System;
using System.Collections.ObjectModel;
using Localization.Strazh.Common;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DeviceStateViewModel : BaseViewModel
	{
		public SKDDeviceState State { get; private set; }

		public DeviceStateViewModel(SKDDeviceState deviceState)
		{
			State = deviceState;
			StateClasses = new ObservableCollection<XStateClassViewModel>();
			State.StateChanged -= new Action(OnStateChanged);
			State.StateChanged += new Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);

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
				return GetStateName(StateClass, Device);
			}
		}

		public static string GetStateName(XStateClass stateClass, SKDDevice device)
		{
			if (device.DriverType == SKDDriverType.Lock)
			{
				switch(stateClass)
				{
					case XStateClass.Off:
				        return CommonResources.Closed;

					case XStateClass.On:
                        return CommonResources.Opened;
				}
			}

			return stateClass.ToDescription();
		}
	}
}