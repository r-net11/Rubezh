using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI;
using Controls.Converters;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class DeviceStateViewModel : BaseViewModel
	{
		public SKDDeviceState State { get; private set; }

		public DeviceStateViewModel(SKDDeviceState deviceState)
		{
			State = deviceState;
			StateClasses = new ObservableCollection<XStateClassViewModel>();
			AdditionalStates = new ObservableCollection<XAdditionalState>();
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

			AdditionalStates.Clear();
			foreach (var additionalState in State.AdditionalStates)
			{
				AdditionalStates.Add(additionalState);
			}
		}

		public ObservableCollection<XStateClassViewModel> StateClasses { get; private set; }
		public ObservableCollection<XAdditionalState> AdditionalStates { get; private set; }
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