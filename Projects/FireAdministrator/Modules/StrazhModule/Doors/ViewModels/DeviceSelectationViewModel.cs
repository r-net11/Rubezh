using System;
using System.Collections.ObjectModel;
using System.Linq;
using Localization.Strazh.ViewModels;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectationViewModel(Guid deviceUID, DoorType doorType)
		{
			Title = CommonViewModels.Device_Selectation;
			Devices = new ObservableCollection<SKDDevice>();
			foreach (var skdDevice in SKDManager.Devices)
			{
				if (skdDevice.DriverType == SKDDriverType.Reader)
				{
					if (skdDevice.Parent != null && skdDevice.Parent.DoorType == doorType)
					{
						if (doorType == DoorType.TwoWay)
						{
							if (skdDevice.IntAddress % 2 == 1)
								continue;
							var outReader = SKDManager.Devices.FirstOrDefault(x => x.Parent != null && skdDevice.Parent != null && x.Parent.UID == skdDevice.Parent.UID && x.IntAddress == skdDevice.IntAddress + 1);
							if (outReader == null)
								continue;
							if (outReader.Door != null)
								continue;
						}

						if (skdDevice.Door != null)
							continue;

						Devices.Add(skdDevice);
					}
				}
			}
			SelectedDevice = Devices.FirstOrDefault(x => x.UID == deviceUID);
		}

		public ObservableCollection<SKDDevice> Devices { get; private set; }

		SKDDevice _selectedDevice;
		public SKDDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		protected override bool Save()
		{
			return SelectedDevice != null;
		}
	}
}