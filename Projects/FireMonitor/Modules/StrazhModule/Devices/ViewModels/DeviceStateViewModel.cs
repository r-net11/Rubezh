using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Devices;

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
						return "Закрыто";

					case XStateClass.On:
						return "Открыто";
				}
			}

			return stateClass.ToDescription();
		}
	}
}