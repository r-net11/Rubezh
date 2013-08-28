using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		DeviceViewModel ParentDeviceViewModel;
		XDevice ParentDevice;
		XDevice RealParentDevice;
		public DeviceViewModel AddedDevice { get; private set; }

		public NewDeviceViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Новое устройство";
			ParentDeviceViewModel = deviceViewModel;
			ParentDevice = ParentDeviceViewModel.Device;
			RealParentDevice = ParentDevice;
			if (ParentDevice.IsConnectedToKAURSR2)
			{
				RealParentDevice = ParentDevice.KAURSR2Parent;
			}

			AvailableShleifs = new ObservableCollection<byte>();
			Drivers = new ObservableCollection<XDriver>();
			foreach (var driver in XManager.DriversConfiguration.XDrivers)
			{
				if (driver.DriverType == XDriverType.AM1_O || driver.DriverType == XDriverType.AMP_1)
					continue;
				if (RealParentDevice.Driver.Children.Contains(driver.DriverType))
					Drivers.Add(driver);
			}

			if (deviceViewModel.Driver.DriverType == XDriverType.MPT)
				Drivers = new ObservableCollection<XDriver>(
					from XDriver driver in XManager.DriversConfiguration.XDrivers
					where driver.DriverType == XDriverType.MPT
					select driver);

			SelectedDriver = Drivers.FirstOrDefault();
			Count = 1;
		}

		public ObservableCollection<XDriver> Drivers { get; private set; }

		XDriver _selectedDriver;
		public XDriver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				UpdateShleif();
				OnPropertyChanged("SelectedDriver");
			}
		}

		void UpdateShleif()
		{
			AvailableShleifs.Clear();
			if (ParentDevice != null)
			{
				var parentShleif = ParentDevice;
				if (ParentDevice.Driver.DriverType == XDriverType.MPT)
					parentShleif = ParentDevice.Parent;
				if (parentShleif.Driver.IsKauOrRSR2Kau)
				{
					for (byte i = 1; i <= 8; i++)
					{
						AvailableShleifs.Add(i);
					}
				}
				else
				{
					if (ParentDevice.IsConnectedToKAURSR2)
					{
						AvailableShleifs.Add(ParentDevice.ShleifNo);
					}
					else
					{
						AvailableShleifs.Add(1);
					}
				}
			}
			SelectedShleif = AvailableShleifs.FirstOrDefault();
		}

		public ObservableCollection<byte> AvailableShleifs { get; private set; }

		byte _selectedShleif;
		public byte SelectedShleif
		{
			get { return _selectedShleif; }
			set
			{
				_selectedShleif = value;
				OnPropertyChanged("SelectedShleif");
				UpdateAddressRange();
			}
		}

		int _startAddress;
		public int StartAddress
		{
			get { return _startAddress; }
			set
			{
				if (_startAddress != value)
				{
					_startAddress = value;
					OnPropertyChanged("StartAddress");
				}
			}
		}

		public bool HasStartAddress
		{
			get
			{
				if (SelectedDriver.DriverType != XDriverType.RSR2_KAU && ParentDevice.IsConnectedToKAURSR2)
					return false;
				return true;
			}
		}

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

		void UpdateAddressRange()
		{
			int maxAddress = NewDeviceHelper.GetMinAddress(SelectedDriver, RealParentDevice, SelectedShleif);
			StartAddress = (byte)(maxAddress % 256);
		}

		bool CreateDevices()
		{
			var step = Math.Max(SelectedDriver.GroupDeviceChildrenCount, (byte)1);

			if (ParentDevice.IsConnectedToKAURSR2)
			{
				for (int i = 0; i < Count; i++)
				{
					var address = StartAddress + i * step;
					if (address + SelectedDriver.GroupDeviceChildrenCount >= 256)
					{
						return true;
					}

					if (ParentDevice.Driver.DriverType == XDriverType.RSR2_KAU)
					{
						var previousDevice = ParentDevice.Children.LastOrDefault();
						var minDelta = 8*256;
						foreach (var child in ParentDevice.Children)
						{
							var delta = (SelectedShleif + 1) * 256 - (child.ShleifNo * 256 + child.IntAddress);
							if (delta > 0 && delta < minDelta)
							{
								minDelta = delta;
								previousDevice = child;
							}
						}

						var previousDeviceViewModel = ParentDeviceViewModel.Children.FirstOrDefault(x => x.Device == previousDevice);
						XDevice device = XManager.InsertChild(ParentDevice, previousDevice, SelectedDriver, SelectedShleif, (byte)address);
						AddedDevice = NewDeviceHelper.InsertDevice(device, previousDeviceViewModel);
					}
					else
					{
						XDevice device = XManager.InsertChild(RealParentDevice, ParentDevice, SelectedDriver, SelectedShleif, (byte)address);
						AddedDevice = NewDeviceHelper.InsertDevice(device, ParentDeviceViewModel);
					}
				}
				XManager.RebuildRSR2Addresses(ParentDevice.KAURSR2Parent);
				return true;
			}
			else
			{
				for (int i = StartAddress; i < StartAddress + Count * step; i++)
				{
					if (ParentDevice.Children.Any(x => x.IntAddress == i && x.ShleifNo == SelectedShleif))
					{
						MessageBoxService.ShowWarning("В заданном диапазоне уже существуют устройства");
						return false;
					}
				}

				if (ParentDevice.Driver.IsGroupDevice)
				{
					Count = Math.Min(Count, ParentDevice.Driver.GroupDeviceChildrenCount);
				}

				for (int i = 0; i < Count; i++)
				{
					var address = StartAddress + i * step;
					if (address + SelectedDriver.GroupDeviceChildrenCount >= 256)
					{
						return true;
					}

					XDevice device = XManager.AddChild(ParentDevice, SelectedDriver, SelectedShleif, (byte)address);
					AddedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
				}
				return true;
			}
		}

		protected override bool CanSave()
		{
			return (SelectedDriver != null);
		}

		protected override bool Save()
		{
			var result = CreateDevices();
			if (result)
			{
				ParentDeviceViewModel.Update();
				XManager.DeviceConfiguration.Update();
			}
			return result;
		}
	}
}