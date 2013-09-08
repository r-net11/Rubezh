using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class RSR2NewDeviceViewModel : NewDeviceViewModelBase
	{
		XDevice RealParentDevice;

		public RSR2NewDeviceViewModel(DeviceViewModel deviceViewModel)
			: base(deviceViewModel)
		{
			RealParentDevice = ParentDevice.KAURSR2Parent;

			foreach (var driver in XManager.Drivers)
			{
				if (RealParentDevice.Driver.Children.Contains(driver.DriverType))
					Drivers.Add(driver);
			}

			SelectedDriver = Drivers.FirstOrDefault();
		}

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
					AvailableShleifs.Add(ParentDevice.ShleifNo);
				}
			}
			SelectedShleif = AvailableShleifs.FirstOrDefault();
		}

		byte _selectedShleif;
		public byte SelectedShleif
		{
			get { return _selectedShleif; }
			set
			{
				_selectedShleif = value;
				OnPropertyChanged("SelectedShleif");
			}
		}

		bool CreateDevices()
		{
			int maxAddress = NewDeviceHelper.GetMinAddress(SelectedDriver, RealParentDevice, SelectedShleif);
			var startAddress = (byte)(maxAddress % 256);
			var step = Math.Max(SelectedDriver.GroupDeviceChildrenCount, (byte)1);

			for (int i = 0; i < Count; i++)
			{
				var address = startAddress + i * step;
				if (address + SelectedDriver.GroupDeviceChildrenCount >= 256)
				{
					return true;
				}

				if (ParentDevice.Driver.DriverType == XDriverType.RSR2_KAU)
				{
					var previousDevice = ParentDevice.Children.LastOrDefault();
					var minDelta = 8 * 256;
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