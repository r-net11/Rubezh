using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Practices.ObjectBuilder2;
using XFiresecAPI;
using System.Collections;

namespace GKModule.ViewModels
{
	public class ConfigurationCompareViewModel : DialogViewModel
	{
		XDevice LocalDevice { get; set; }
		XDevice RemoteDevice { get; set; }
		XDeviceConfiguration LocalConfiguration { get; set; }
		XDeviceConfiguration RemoteConfiguration { get; set; }
		
		public ConfigurationCompareViewModel(XDeviceConfiguration localConfiguration, XDeviceConfiguration remoteConfiguration, XDevice device)
		{
			Title = "Сравнение конфигураций";
			ChangeCommand = new RelayCommand(OnChange);
			NextDifferenceCommand = new RelayCommand(OnNextDifference, CanNextDifference);
			PreviousDifferenceCommand = new RelayCommand(OnPreviousDifference, CanPreviousDifference);

			LocalConfiguration = localConfiguration;
			RemoteConfiguration = remoteConfiguration;

			LocalDevice = localConfiguration.Devices.FirstOrDefault(x => x.DriverType == device.DriverType&& x.Address == device.Address);
			RemoteDevice = remoteConfiguration.Devices.FirstOrDefault(x => x.DriverType == device.DriverType && x.Address == device.Address);

			var localDeviceClone = (XDevice) LocalDevice.Clone();
			var remoteDeviceClone = (XDevice) RemoteDevice.Clone();

			if (device.DriverType == XDriverType.GK)
			{
				LocalDeviceViewModel = new ObjectsListViewModel(localDeviceClone, localConfiguration.Zones,localConfiguration.Directions);
				RemoteDeviceViewModel = new ObjectsListViewModel(remoteDeviceClone, remoteConfiguration.Zones,remoteConfiguration.Directions);
			}
			else
			{
				LocalDeviceViewModel = new ObjectsListViewModel(localDeviceClone, null, null);
				RemoteDeviceViewModel = new ObjectsListViewModel(remoteDeviceClone, null, null);
			}
            var compareDevices = ObjectsListViewModel.CompareTrees(LocalDeviceViewModel.Devices, RemoteDeviceViewModel.Devices, device.DriverType);
			LocalDeviceViewModel.Devices = compareDevices[0];
			RemoteDeviceViewModel.Devices = compareDevices[1];
            if (device.DriverType == XDriverType.GK)
            {
                var compareZones = ObjectsListViewModel.CompareTrees(LocalDeviceViewModel.Zones, RemoteDeviceViewModel.Zones, device.DriverType);
                LocalDeviceViewModel.Zones = compareZones[0];
                RemoteDeviceViewModel.Zones = compareZones[1];
                var compareDirections = ObjectsListViewModel.CompareTrees(LocalDeviceViewModel.Directions, RemoteDeviceViewModel.Directions, device.DriverType);
                LocalDeviceViewModel.Directions = compareDirections[0];
                RemoteDeviceViewModel.Directions = compareDirections[1];
            }
			InitializeMismatchedIndexes();
		}
		public ObjectsListViewModel LocalDeviceViewModel { get; set; }
		public ObjectsListViewModel RemoteDeviceViewModel { get; set; }
		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			RemoteDevice.UID = LocalDevice.UID;
			var rootDevice = LocalConfiguration.Devices.FirstOrDefault(x => x.UID == LocalDevice.Parent.UID);
			rootDevice.Children.Remove(LocalDevice);
			rootDevice.Children.Add(RemoteDevice);
			if(RemoteDevice.DriverType == XDriverType.GK)
			{
				foreach (var kauChild in RemoteDevice.Children)
				{
					if(kauChild.Driver.IsKauOrRSR2Kau)
						AddShleifs(kauChild);
				}
			}
			if(RemoteDevice.Driver.IsKauOrRSR2Kau)
				AddShleifs(RemoteDevice);
			if (LocalDevice.DriverType == XDriverType.GK)
			{
				LocalConfiguration.Zones = RemoteConfiguration.Zones;
				LocalConfiguration.Directions = RemoteConfiguration.Directions;
			}
			ServiceFactory.SaveService.GKChanged = true;
			XManager.UpdateConfiguration();
			Close(true);
		}

		List<int> mismatchedIndexes;
		void InitializeMismatchedIndexes()
		{
			mismatchedIndexes = new List<int>();
			foreach (var item in LocalDeviceViewModel.Objects)
			{
				var itemIndex = LocalDeviceViewModel.Objects.IndexOf(item);
				if ((item.HasDifferences || RemoteDeviceViewModel.Objects[itemIndex].HasDifferences)&&(!mismatchedIndexes.Contains(itemIndex)))
					mismatchedIndexes.Add(itemIndex);
			}
		}

		int selectedIndex
		{
			get { return LocalDeviceViewModel.Objects.IndexOf(LocalDeviceViewModel.SelectedObject); }
		}

		public RelayCommand NextDifferenceCommand { get; private set; }
		void OnNextDifference()
		{
			var nextIndex = mismatchedIndexes.FirstOrDefault(x => x > selectedIndex);
			LocalDeviceViewModel.SelectedObject = LocalDeviceViewModel.Objects[nextIndex];
		}
		bool CanNextDifference()
		{
			return mismatchedIndexes.Any(x => x > selectedIndex);
		}

		public RelayCommand PreviousDifferenceCommand { get; private set; }
		void OnPreviousDifference()
		{
			var previousIndex = mismatchedIndexes.LastOrDefault(x => x < selectedIndex);
			LocalDeviceViewModel.SelectedObject = LocalDeviceViewModel.Objects[previousIndex];
		}
		bool CanPreviousDifference()
		{
			return mismatchedIndexes.Any(x => x < selectedIndex);
		}

		static void AddShleifs(XDevice device)
		{
			var deviceChildren = new List<XDevice>(device.Children);
			device.Children = new List<XDevice>();
			for (int i = 0; i < 8; i++)
			{
				var shleif = new XDevice();
				shleif.Driver = device.Driver.DriverType == XDriverType.KAU ? XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.KAU_Shleif) : XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU_Shleif);
				shleif.DriverUID = shleif.Driver.UID;
				shleif.IntAddress = (byte)(i + 1);
				device.Children.Add(shleif);
			}
			foreach (var child in deviceChildren)
			{
				if ((1 <= child.ShleifNo) && (child.ShleifNo <= 8))
				{
					var shleif = device.Children.FirstOrDefault(x => ((x.DriverType == XDriverType.KAU_Shleif) || (x.DriverType == XDriverType.RSR2_KAU_Shleif)) && x.IntAddress == child.ShleifNo);
					shleif.Children.Add(child);
					child.Parent = shleif;
				}
			}
		}
	}
}