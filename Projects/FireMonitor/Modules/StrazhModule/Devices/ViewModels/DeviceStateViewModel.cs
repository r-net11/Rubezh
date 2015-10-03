using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
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

		public bool IsPromptWarning
		{
			get
			{
				//return StateClasses.Any(x => x.StateClass == XStateClass.Attention);
				return State.Device.DriverType == SKDDriverType.Lock;
			}
		}

		public DeviceStateViewModel(SKDDeviceState deviceState)
		{
			ClearPromptWarningCommand = new RelayCommand(OnClearPromptWarning, CanClearPromptWarning);
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
			OnPropertyChanged(() => IsPromptWarning);
		}

		public ObservableCollection<XStateClassViewModel> StateClasses { get; private set; }

		public RelayCommand ClearPromptWarningCommand { get; private set; }
		private void OnClearPromptWarning()
		{
			DeviceCommander.ClearPromptWarning(State.Device);
		}

		private bool CanClearPromptWarning()
		{
			return DeviceCommander.CanClearPromptWarning(State.Device);
		}
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
			if (stateClass == XStateClass.Attention)
				return "Взлом";

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