using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Firesec.Imitator.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
		public DeviceViewModel(DeviceState deviceState)
		{
			DeviceState = deviceState;
			Name = DeviceState.Device.DottedPresentationAddressAndName;
			Level = DeviceState.Device.AllParents.Count;
			ImageSource = DeviceState.Device.Driver.ImageSource;

			DriverStates = new List<DeviceStateViewModel>();
			foreach (var driverState in from x in DeviceState.Device.Driver.States orderby x.StateType select x)
			{
				if (!string.IsNullOrEmpty(driverState.Name))
				{
					var deviceStateViewModel = new DeviceStateViewModel(driverState);
					DriverStates.Add(deviceStateViewModel);
				}
			}

			foreach (var deviceDriverState in deviceState.ThreadSafeStates)
			{
				var state = DriverStates.FirstOrDefault(x => x.DriverState.Code == deviceDriverState.DriverState.Code);
				state._isActive = true;
			}
		}

		public DeviceState DeviceState { get; private set; }
		public string Name { get; private set; }
		public List<DeviceStateViewModel> DriverStates { get; private set; }
		public int Level { get; private set; }
		public string ImageSource { get; private set; }
		public StateType StateType
		{
			get { return DeviceState.StateType; }
		}
	}
}