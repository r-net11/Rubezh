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

namespace StrazhModule.ViewModels
{
	public class DeviceStateViewModel : BaseViewModel
	{
		public SKDDeviceState State { get; private set; }

		public bool IsPromptWarning
		{
			get { return StateClasses.Any(x => x.StateClass == XStateClass.Attention); }
		}

		public DeviceStateViewModel(SKDDeviceState deviceState)
		{
			ClearDevicePromptWarningCommand = new RelayCommand(OnClearDevicePromptWarning/*, CanClearDevicePromptWarning*/);
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

		public RelayCommand ClearDevicePromptWarningCommand { get; private set; }
		public void OnClearDevicePromptWarning()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDClearDevicePromptWarning(State.Device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		//public bool CanClearDevicePromptWarning()
		//{
		//	return StateClasses.Any(x => x.StateClass == XStateClass.Attention);
		//}
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