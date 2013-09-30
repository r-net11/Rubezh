using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Linq;

namespace GKModule.ViewModels
{
	public class DeviceConfigurationViewModel : DialogViewModel
	{
		XDevice KauDevice;
		List<XDevice> ChildDevices;
		XDevice LocalRootClone;
		XDevice RemoteRootClone;

		public DeviceConfigurationViewModel(XDevice kauDevice, List<XDevice> devices)
		{
			Title = "Конфигурация устройств";
			KauDevice = kauDevice;
			ChildDevices = devices;
			LocalDevice = new DeviceTreeViewModel();
			RemoteDevice = new DeviceTreeViewModel();

			LocalRootClone = (XDevice)kauDevice.Clone();
			RemoteRootClone = (XDevice)kauDevice.Clone();
			RemoteRootClone.Children = new List<XDevice>(devices);

			LocalRootClone.Children = new List<XDevice>();
			if (kauDevice.Children != null)
				foreach (var children in kauDevice.Children)
				{
					var childrenClone = (XDevice)children.Clone();
					LocalRootClone.Children.Add(childrenClone);
					if (children.Children != null)
					{
						var localchch =
							LocalRootClone.Children.FirstOrDefault(
								x =>
								((x.PresentationAddressAndDriver == children.PresentationAddressAndDriver) &&
								 (x.PresentationAddress == children.PresentationAddress)));
						localchch.Children = new List<XDevice>();
						foreach (var chch in children.Children)
						{
							var chchClone = (XDevice)chch.Clone();
							localchch.Children.Add(chchClone);
						}
					}
				}

			RemoteRootClone.Children = new List<XDevice>();
			if (devices != null)
				foreach (var children in devices)
				{
					var childrenClone = (XDevice)children.Clone();
					RemoteRootClone.Children.Add(childrenClone);
					if (children.Children != null)
					{
						var remotechch =
							RemoteRootClone.Children.FirstOrDefault(
								x =>
								((x.PresentationAddressAndDriver == children.PresentationAddressAndDriver) &&
								 (x.PresentationAddress == children.PresentationAddress)));
						remotechch.Children = new List<XDevice>();
						foreach (var chch in children.Children)
						{
							var chchClone = (XDevice)chch.Clone();
							remotechch.Children.Add(chchClone);
						}
					}
				}

			IntoLocalDevice(kauDevice, RemoteRootClone);
			IntoRemoteDevice(RemoteRootClone, LocalRootClone);

			Sort(LocalRootClone);
			Sort(RemoteRootClone);

			LocalDevice.Devices = new ObservableCollection<XDevice>(LocalRootClone.Children);
			RemoteDevice.Devices = new ObservableCollection<XDevice>(RemoteRootClone.Children);

			ChangeCommand = new RelayCommand(OnChange);
		}

		void IntoLocalDevice(XDevice localRootDevice, XDevice remoteRootDevice)
		{
			//remoteRootDevice.DeviceConfiguration = RemoteDeviceConfiguration;
			foreach (var localDevice in localRootDevice.Children)
			{
				var remoteAndLocal =
					remoteRootDevice.Children.FirstOrDefault(x => (x.Driver.ShortName == localDevice.Driver.ShortName) && (x.Address == localDevice.Address));
				if (remoteAndLocal == null)
				{
					var remoteDevice = (XDevice)localDevice.Clone();
					remoteDevice.Children = new List<XDevice>();
					remoteDevice.HasMissingDifferences = true;
					remoteAndLocal = remoteDevice;
					//remoteAndLocal.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
					remoteRootDevice.Children.Add(remoteDevice);
				}
				else
				{
					if (remoteAndLocal.Zones == null && localDevice.Zones != null)
					{
						remoteAndLocal.Zones = localDevice.Zones;
						remoteAndLocal.HasDifferences = true;
						//remoteAndLocal.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
					}
					//else if (remoteAndLocal.ZonesInLogic.Count == 0 && localDevice.ZonesInLogic.Count != 0)
					//{
					//    remoteAndLocal.ZonesInLogic = localDevice.ZonesInLogic;
					//    remoteAndLocal.ZoneLogic = localDevice.ZoneLogic;
					//    remoteAndLocal.HasDifferences = true;
					//    remoteAndLocal.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
					//}
					else
					{
						remoteAndLocal.HasDifferences = false;
						//remoteAndLocal.DeviceConfiguration = RemoteDeviceConfiguration;
					}
				}

				if ((localDevice.Children != null) && (localDevice.Children.Count > 0))
				{
					IntoLocalDevice(localDevice, remoteAndLocal);
				}
			}
		}

		void IntoRemoteDevice(XDevice remoteRootDevice, XDevice localRootDevice)
		{
			//localRootDevice.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
			foreach (var remote in remoteRootDevice.Children)
			{
				var localAndRemote = localRootDevice.Children.FirstOrDefault(x => (x.Driver.ShortName == remote.Driver.ShortName) && (x.Address == remote.Address));
				if (localAndRemote == null)
				{
					var local = (XDevice)remote.Clone();
					local.Children = new List<XDevice>();
					local.HasMissingDifferences = true;
					localAndRemote = local;
					//localAndRemote.DeviceConfiguration = RemoteDeviceConfiguration;
					localRootDevice.Children.Add(local);
				}
				else
				{
					if (localAndRemote.Zones == null && remote.Zones != null)
					{
						localAndRemote.HasDifferences = true;
						//localAndRemote.DeviceConfiguration = RemoteDeviceConfiguration;
					}
					//if (localAndRemote.Zone != null && remote.Zone != null && localAndRemote.Zone.PresentationName != remote.Zone.PresentationName)
					//{
					//    localAndRemote.HasDifferences = true;
					//    localAndRemote.DeviceConfiguration = RemoteDeviceConfiguration;
					//}
					//else if (localAndRemote.ZonesInLogic.Count == 0 && remote.ZonesInLogic.Count != 0)
					//{
					//    localAndRemote.HasDifferences = true;
					//    localAndRemote.DeviceConfiguration = RemoteDeviceConfiguration;
					//}
					else
					{
						localAndRemote.HasDifferences = false;
						//localAndRemote.DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;
					}
				}
				if ((remote.Children != null) && (remote.Children.Count > 0))
				{
					IntoRemoteDevice(remote, localAndRemote);
				}
			}
		}

		void Sort(XDevice device)
		{
			if ((device.Children != null)&&(device.Children.Count != 0))
			{
				device.Children = device.Children.OrderByDescending(x => x.IntAddress).ToList();
			}
			foreach (var child in device.Children)
			{
				if ((child.Children != null)&&(child.Children.Count != 0))
					Sort(child);
			}
		}

		public DeviceTreeViewModel LocalDevice { get; set; }
		public DeviceTreeViewModel RemoteDevice { get; set; }
		//public ObservableCollection<XDevice> Devices { get; private set; }

		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			ChildDevices.RemoveAll(x => x.Driver.IsKauOrRSR2Kau);
			KauDevice.Children = new List<XDevice>();
			KauDevice.Children.AddRange(ChildDevices);
			ServiceFactory.SaveService.GKChanged = true;
			Close(true);
		}
	}
}