using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DeviceStateViewModel : BaseViewModel
	{
		public GKState State { get; private set; }
		public bool IsStateImage { get; private set; }

		public DeviceStateViewModel(GKState deviceState, bool isStateImage)
		{
			State = deviceState;
			IsStateImage = isStateImage;
			StateClasses = new ObservableCollection<GKStateClassViewModel>();
			AdditionalStates = new ObservableCollection<GKAdditionalState>();
			State.StateChanged += OnStateChanged;
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);

			StateClasses.Clear();
			foreach (var stateClass in State.StateClasses)
			{
				StateClasses.Add(new GKStateClassViewModel(State.Device, stateClass));
			}

			AdditionalStates.Clear();
			foreach (var additionalState in State.AdditionalStates)
			{
				AdditionalStates.Add(additionalState);
			}
		}

		public ObservableCollection<GKStateClassViewModel> StateClasses { get; private set; }
		public ObservableCollection<GKAdditionalState> AdditionalStates { get; private set; }
	}

	public class GKStateClassViewModel : BaseViewModel
	{
		public XStateClass StateClass { get; private set; }
		GKDevice Device { get; set; }

		public GKStateClassViewModel(GKDevice device, XStateClass stateClass)
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

		public static string GetStateName(XStateClass stateClass, GKDevice device)
		{
			if (device != null)
			{
				if (device.DriverType == GKDriverType.RSR2_Valve_KVMV || device.DriverType == GKDriverType.RSR2_Valve_DU)
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
				if (device.DriverType == GKDriverType.RSR2_AM_1)
				{
					if (stateClass == XStateClass.Norm)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "Сообщение для нормы");
						if (property != null)
						{
							if (!string.IsNullOrEmpty(property.StringValue))
								return property.StringValue;
						}
					}
					if (stateClass == XStateClass.Fire1)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "Сообщение для сработки 1");
						if (property != null)
						{
							if (!string.IsNullOrEmpty(property.StringValue))
								return property.StringValue;
						}
					}
					if (stateClass == XStateClass.Fire2)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "Сообщение для сработки 2");
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