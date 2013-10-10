using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using Controls.Converters;

namespace GKModule.ViewModels
{
	public class DeviceTooltipViewModel : BaseViewModel
	{
		public XDeviceState DeviceState { get; private set; }
		public XDevice Device { get; private set; }

		public DeviceTooltipViewModel(XDevice device)
		{
			Device = device;
			DeviceState = device.DeviceState;
			StateClasses = new ObservableCollection<XStateClassViewModel>();
		}

		public void OnStateChanged()
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
		}

		public ObservableCollection<XStateClassViewModel> StateClasses { get; private set; }

		public string StateClassName
		{
			get
			{
				var converter = new XStateClassToDeviceStringConverter();
				var result = (string)converter.Convert(DeviceState.StateClass, null, DeviceState.Device, null);
				return result;
			}
		}
	}
}