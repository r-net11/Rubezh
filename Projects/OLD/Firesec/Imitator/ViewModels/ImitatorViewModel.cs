using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Firesec.Models.CoreState;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Firesec.Imitator.ViewModels
{
	public class ImitatorViewModel : DialogViewModel
	{
		internal static ImitatorViewModel Current { get; private set; }

		public ImitatorViewModel()
		{
			Current = this;

			Title = "Имитатор состояний устройств";
			Devices = new ObservableCollection<DeviceViewModel>();

			foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == DriverType.IndicationBlock)
					continue;
				if (device.Driver.DriverType == DriverType.Page)
					continue;
				if (device.Driver.DriverType == DriverType.Indicator)
					continue;

				var deviceViewModel = new DeviceViewModel(device.DeviceState);
				Devices.Add(deviceViewModel);
			}

			if (Watcher.Current != null)
			{
				Watcher.Current.DevicesStateChanged += new System.Action<List<FiresecAPI.Models.DeviceState>>(OnDevicesStateChanged);
			}
		}

		void OnDevicesStateChanged(List<DeviceState> deviceStates)
		{
			foreach (var deviceState in deviceStates)
			{
				var device = Devices.FirstOrDefault(x => x.DeviceState == deviceState);
				if (device != null)
				{
				}
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public void Update()
		{
			var coreStates = new Firesec.Models.CoreState.States();
			var innerDevices = new List<devType>();
			foreach (var device in Devices)
			{
				var innerDevice = new devType()
				{
					name = device.DeviceState.Device.PlaceInTree
				};
				var innerStates = new List<stateType>();
				foreach (var state in device.DriverStates)
				{
					if (state.IsActive)
					{
						var innerState = new stateType()
						{
							id = state.DriverState.Id
						};
						innerStates.Add(innerState);
					}
				}
				innerDevice.state = innerStates.ToArray();
				innerDevices.Add(innerDevice);
			}
			coreStates.dev = innerDevices.ToArray();
			Watcher.ImitatorStateChanged(coreStates);
		}
	}
}