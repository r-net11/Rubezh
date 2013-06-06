using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;

namespace MonitorClientFS2.ViewModels
{
	public class DeviceDetailsViewModel : DialogViewModel
	{
		public Device Device { get; private set; }
		public DeviceState DeviceState { get; private set; }

		public DeviceDetailsViewModel(Device device)
		{
			Device = device;
			DeviceState = Device.DeviceState;
			DeviceState.StateChanged += new Action(OnStateChanged);
			DeviceState.ParametersChanged += new Action(OnParametersChanged);
			OnStateChanged();

			Title = Device.DottedPresentationAddressAndName;
			TopMost = true;
		}

		void OnStateChanged()
		{
		}

		void OnParametersChanged()
		{
		}
	}
}