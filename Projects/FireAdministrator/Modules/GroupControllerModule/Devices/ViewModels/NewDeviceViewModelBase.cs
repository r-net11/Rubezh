using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class NewDeviceViewModelBase : SaveCancelDialogViewModel
	{
		public NewDeviceViewModelBase(DeviceViewModel deviceViewModel)
		{
			Title = "Новое устройство";
			ParentDeviceViewModel = deviceViewModel;
			ParentDevice = ParentDeviceViewModel.Device;
			Drivers = new ObservableCollection<GKDriver>();
			AvailableShleifs = new ObservableCollection<byte>();
			AddedDevices = new List<DeviceViewModel>();
			Count = 1;
		}

		protected DeviceViewModel ParentDeviceViewModel;
		protected GKDevice ParentDevice;
		public List<DeviceViewModel> AddedDevices { get; protected set; }
		public ObservableCollection<GKDriver> Drivers { get; protected set; }
		public ObservableCollection<byte> AvailableShleifs { get; protected set; }

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

		protected List<GKDriver> SortDrivers()
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
			var sortedDrivers = from DriverCounter driverCounter in driverCounters orderby driverCounter.Count descending select driverCounter.Driver;
			return sortedDrivers.ToList();
		}

		class DriverCounter
		{
			public GKDriver Driver { get; set; }
			public int Count { get; set; }
		}
	}
}