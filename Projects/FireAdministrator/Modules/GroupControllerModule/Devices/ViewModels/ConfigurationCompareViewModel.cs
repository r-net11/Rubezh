using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ConfigurationCompareViewModel : DialogViewModel
	{
		XDevice LocalDevice { get; set; }
		XDevice RemoteDevice { get; set; }
		XDeviceConfiguration LocalConfiguration { get; set; }
		XDeviceConfiguration RemoteConfiguration { get; set; }
		public ObjectsListViewModel LocalObjectsViewModel { get; set; }
		public ObjectsListViewModel RemoteObjectsViewModel { get; set; }
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

			LocalObjectsViewModel = new ObjectsListViewModel(LocalDevice, localConfiguration);
			RemoteObjectsViewModel = new ObjectsListViewModel(RemoteDevice, remoteConfiguration);

			var compareDevices = ObjectsListViewModel.CompareTrees(LocalObjectsViewModel.Devices, RemoteObjectsViewModel.Devices, device.DriverType);
			LocalObjectsViewModel.Devices = compareDevices[0];
			RemoteObjectsViewModel.Devices = compareDevices[1];
			if (device.DriverType == XDriverType.GK)
			{
				var compareZones = ObjectsListViewModel.CompareTrees(LocalObjectsViewModel.Zones, RemoteObjectsViewModel.Zones, device.DriverType);
				LocalObjectsViewModel.Zones = compareZones[0];
				RemoteObjectsViewModel.Zones = compareZones[1];
				var compareDirections = ObjectsListViewModel.CompareTrees(LocalObjectsViewModel.Directions, RemoteObjectsViewModel.Directions, device.DriverType);
				LocalObjectsViewModel.Directions = compareDirections[0];
				RemoteObjectsViewModel.Directions = compareDirections[1];
			}
			InitializeMismatchedIndexes();
		}
		
		List<int> mismatchedIndexes;
		void InitializeMismatchedIndexes()
		{
			mismatchedIndexes = new List<int>();
			for (int i = 0; i < LocalObjectsViewModel.Objects.Count; i++)
			{
				var item = LocalObjectsViewModel.Objects[i];
				if ((item.HasDifferences || RemoteObjectsViewModel.Objects[i].HasDifferences) && !mismatchedIndexes.Contains(i))
					mismatchedIndexes.Add(i);
			}
		}

		int SelectedIndex
		{
			get { return LocalObjectsViewModel.Objects.IndexOf(LocalObjectsViewModel.SelectedObject); }
		}

		public RelayCommand NextDifferenceCommand { get; private set; }
		void OnNextDifference()
		{
			var nextIndex = mismatchedIndexes.FirstOrDefault(x => x > SelectedIndex);
			LocalObjectsViewModel.SelectedObject = LocalObjectsViewModel.Objects[nextIndex];
		}
		bool CanNextDifference()
		{
			return mismatchedIndexes.Any(x => x > SelectedIndex);
		}

		public RelayCommand PreviousDifferenceCommand { get; private set; }
		void OnPreviousDifference()
		{
			var previousIndex = mismatchedIndexes.LastOrDefault(x => x < SelectedIndex);
			LocalObjectsViewModel.SelectedObject = LocalObjectsViewModel.Objects[previousIndex];
		}
		bool CanPreviousDifference()
		{
			return mismatchedIndexes.Any(x => x < SelectedIndex);
		}

		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			RemoteDevice.UID = LocalDevice.UID;
			var rootDevice = LocalConfiguration.Devices.FirstOrDefault(x => x.UID == LocalDevice.Parent.UID);
			rootDevice.Children.Remove(LocalDevice);
			rootDevice.Children.Add(RemoteDevice);
			if (RemoteDevice.DriverType == XDriverType.GK)
			{
				foreach (var kauChild in RemoteDevice.Children)
				{
					if (kauChild.Driver.IsKauOrRSR2Kau)
						AddShleifs(kauChild);
				}
			}
			if (RemoteDevice.Driver.IsKauOrRSR2Kau)
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

		void AddShleifs(XDevice device)
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
				if (1 <= child.ShleifNo && child.ShleifNo <= 8)
				{
					var shleif = device.Children.FirstOrDefault(x => ((x.DriverType == XDriverType.KAU_Shleif) || (x.DriverType == XDriverType.RSR2_KAU_Shleif)) && x.IntAddress == child.ShleifNo);
					shleif.Children.Add(child);
					child.Parent = shleif;
				}
			}
		}
	}
}