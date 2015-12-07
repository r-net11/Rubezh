using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using Infrastructure;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public NewDeviceViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Новое устройство";
			ParentDeviceViewModel = deviceViewModel;
			ParentDevice = ParentDeviceViewModel.Device;
			AvailableShleifs = new ObservableCollection<byte>();
			AddedDevices = new List<DeviceViewModel>();
			if (ParentDevice.IsConnectedToKAU)
			{
				RealParentDevice = ParentDevice.MVPPartParent == null ? ParentDevice.KAUShleifParent : ParentDevice.MVPPartParent;
				Drivers = new ObservableCollection<GKDriver>(SortDrivers().Where(x => RealParentDevice.Driver.Children.Contains(x.DriverType)));
			}
			else
				Drivers = new ObservableCollection<GKDriver>(SortDrivers().Where(x => ParentDevice.Driver.Children.Contains(x.DriverType)));

			SelectedDriver = Drivers.FirstOrDefault();
			Count = 1;
		}
		GKDevice RealParentDevice;
	    DeviceViewModel ParentDeviceViewModel;
        GKDevice ParentDevice;
		public List<DeviceViewModel> AddedDevices { get; protected set; }
		public ObservableCollection<GKDriver> Drivers { get; protected set; }
		public ObservableCollection<byte> AvailableShleifs { get; protected set; }
		public int MaxAddress { get; private set;}
		public int MinAdderss { get; private set;}

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged(() => Count);
			}
		}

		GKDriver _selectedDriver;
		public GKDriver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				if (ParentDevice.IsConnectedToKAU)
				{
					MinAdderss = 1;
					MaxAddress = 255;
				}
				else
				{
					MinAdderss = value.MinAddress;
					MaxAddress = value.MaxAddress;
				}
				OnPropertyChanged(() => SelectedDriver);
			}
		}

		public List<GKDriver> SortDrivers()
		{
			var driverCounters = new List<DriverCounter>();
			foreach (var driver in GKManager.Drivers)
			{
				var driverCounter = new DriverCounter()
				{
					Driver = driver,
					Count = 0
				};
				driverCounters.Add(driverCounter);
			}
			foreach (var device in GKManager.Devices)
			{
				var driverCounter = driverCounters.FirstOrDefault(x => x.Driver == device.Driver);
				if (driverCounter != null)
				{
					driverCounter.Count++;
				}
			}
			var sortedDrivers = driverCounters.OrderByDescending(x => x.Count).Select(x => x.Driver);
			return sortedDrivers.ToList();
		}

		public int GetAddress(IEnumerable<GKDevice> children)
		{
			if (children.Count() > 0)
				return children.Max(x => x.IntAddress);
			else
				return 0;
		}

		public  bool CreateDevices() 
		{
			AddedDevices = new List<DeviceViewModel>();
			if (ParentDevice.IsConnectedToKAU)
			{
				var address = GetAddress(RealParentDevice.KAUShleifParent.AllChildren);
				if (address + Count * Math.Max(1, (int)SelectedDriver.GroupDeviceChildrenCount) > 255)
				{
					MessageBoxService.ShowWarning("При добавлении количество устройств на АЛС будет превышать 255");
					return false;
				}

				for (int i = 0; i < Count; i++)
				{
					if (RealParentDevice == ParentDevice)
					{
						GKDevice device = GKManager.AddChild(ParentDevice, null, SelectedDriver, 0);
						var addedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
						AddedDevices.Add(addedDevice);
					}
					else
					{
						GKDevice device = GKManager.AddChild(RealParentDevice, ParentDevice, SelectedDriver, 0);
						var addedDevice = NewDeviceHelper.InsertDevice(device, ParentDeviceViewModel);
						if (AddedDevices.Count == 0)
							AddedDevices.Add(addedDevice);
						else
							AddedDevices.Insert(0, addedDevice);
					}
				}

				GKManager.RebuildRSR2Addresses(ParentDevice);
				//if (RealParentDevice != ParentDevice)
				//	AddedDevices.Reverse();
				return true;
			}
			else
			{
				var address = GetAddress(ParentDevice.Children.Where(x => x.Driver.HasAddress));
				if (Count + address > SelectedDriver.MaxAddress && SelectedDriver.HasAddress)
				{
					ServiceFactory.MessageBoxService.ShowWarning("При добавлении устройств количество будет превышать максимально допустимое значения в " + SelectedDriver.MaxAddress.ToString());
					return false;
				}

				for (int i = 1; i <= Count; i++)
				{
					GKDevice device = GKManager.AddChild(ParentDevice, null, SelectedDriver, address + i);
					var addedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
					AddedDevices.Add(addedDevice);
				}
				return true;
			}
		}

		protected override bool CanSave()
		{
			return SelectedDriver != null;
		}

		protected override bool Save()
		{
			var result = CreateDevices();
			if (result)
			{
				ParentDeviceViewModel.Update();
				GKManager.DeviceConfiguration.Update();
			}
			return result;
		}

		class DriverCounter
		{
			public GKDriver Driver { get; set; }
			public int Count { get; set; }
		}
	}
}