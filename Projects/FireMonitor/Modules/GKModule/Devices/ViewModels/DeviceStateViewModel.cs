using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceStateViewModel : BaseViewModel
	{
		public XState State { get; private set; }

		public DeviceStateViewModel(XState deviceState)
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