using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class StateViewModel : BaseViewModel
	{
		public StateViewModel(DriverState driverState, Device device = null)
		{
			DriverState = driverState;
			Name = driverState.Name;
			if (device != null && device.Driver.DriverType == DriverType.AM1_T)
			{
				switch (driverState.Name)
				{
					case "Состояние 1":
						var property = device.Properties.FirstOrDefault(x => x.Name == "Event1");
						if (property != null)
							Name = property.Value;
						break;

					case "Состояние 2":
						property = device.Properties.FirstOrDefault(x => x.Name == "Event2");
						if (property != null)
							Name = property.Value;
						break;
				}
			}
		}

		public string DeviceName { get; set; }
		public DriverState DriverState { get; private set; }
		public StateType StateType
		{
			get { return DriverState.StateType; }
		}
		public string Name { get; private set; }
	}
}