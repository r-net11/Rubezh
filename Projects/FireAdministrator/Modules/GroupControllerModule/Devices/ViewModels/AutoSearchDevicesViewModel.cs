using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure;
using FiresecClient;
using Infrastructure.Events;
using Infrastructure.Common.Services;

namespace GKModule.ViewModels
{
	public class AutoSearchDevicesViewModel : DialogViewModel
	{
		GKDevice LocalDevice;
		GKDeviceConfiguration RemoteDeviceConfiguration;

		public AutoSearchDevicesViewModel(GKDeviceConfiguration deviceConfiguration, GKDevice localDevice)
		{
			Title = "Устройства, найденные в результате автопоиска";
			ChangeCommand = new RelayCommand(OnChange, CanChange);
			RemoteDeviceConfiguration = deviceConfiguration;
			LocalDevice = localDevice;

			deviceConfiguration.UpdateConfiguration();
		}

		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			var RemoteDevice = RemoteDeviceConfiguration.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			var LocalConfiguration = GKManager.DeviceConfiguration;

			LocalDevice.Children.RemoveAll(x => x.DriverType == GKDriverType.RSR2_KAU);
			foreach (var kauChild in RemoteDevice.Children)
			{
				if (kauChild.DriverType == GKDriverType.RSR2_KAU)
				{
					LocalDevice.Children.Add(kauChild);
				}
			}

			LocalConfiguration.Doors.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
			LocalConfiguration.Doors.AddRange(RemoteDeviceConfiguration.Doors);

			ServiceFactory.SaveService.GKChanged = true;
			GKManager.UpdateConfiguration();
			ServiceFactoryBase.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			Close(true);
		}

		bool CanChange()
		{
			return true;
		}
	}
}