using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class NewDeviceViewModelBase : SaveCancelDialogViewModel
	{
		public NewDeviceViewModelBase(DeviceViewModel deviceViewModel)
		{
			Title = "Новое устройство";
			ParentDeviceViewModel = deviceViewModel;
			ParentDevice = ParentDeviceViewModel.Device;
			Drivers = new ObservableCollection<XDriver>();
			AvailableShleifs = new ObservableCollection<byte>();
			Count = 1;
		}

		protected DeviceViewModel ParentDeviceViewModel;
		protected XDevice ParentDevice;
		public DeviceViewModel AddedDevice { get; protected set; }
		public ObservableCollection<XDriver> Drivers { get; protected set; }
		public ObservableCollection<byte> AvailableShleifs { get; protected set; }

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged("Count");
			}
		}

		protected List<XDriver> SortDrivers()
		{
			var driverCounters = new List<DriverCounter>();
			foreach (var driver in XManager.Drivers)
			{
				var driverCounter = new DriverCounter()
				{
					Driver = driver,
					Count = 0
				};
				driverCounters.Add(driverCounter);
			}
			foreach (var device in XManager.Devices)
			{
				var driverCounter = driverCounters.FirstOrDefault(x => x.Driver == device.Driver);
				if (driverCounter != null)
				{
					driverCounter.Count++;
				}
			}
			var sortedDrivers = from DriverCounter driverCounter in driverCounters orderby driverCounter.Count descending select driverCounter.Driver;
			return sortedDrivers.ToList();
		}

		class DriverCounter
		{
			public XDriver Driver { get; set; }
			public int Count { get; set; }
		}
	}
}