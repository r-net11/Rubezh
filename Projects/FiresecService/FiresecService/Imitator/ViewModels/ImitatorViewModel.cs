using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using FiresecService.Service;
using Firesec.CoreState;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace FiresecService.ViewModels
{
	public class ImitatorViewModel : DialogViewModel
	{
		public static ImitatorViewModel Current { get; private set; }

		public ImitatorViewModel()
		{
			Current = this;

			Title = "Имитатор состояний устройств";
			Devices = new ObservableCollection<DeviceViewModel>();

			foreach (var deviceState in ConfigurationCash.DeviceConfigurationStates.DeviceStates)
			{
				if (deviceState.Device.Driver.DriverType == (DriverType.IndicationBlock | DriverType.Page | DriverType.Indicator))
					continue;

				var deviceViewModel = new DeviceViewModel(deviceState);
				Devices.Add(deviceViewModel);
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
				OnPropertyChanged("SelectedDevice");
			}
		}

		public void Update()
		{
			var coreStates = new Firesec.CoreState.config();
			var innerDevices = new List<devType>();
			foreach (var device in Devices)
			{
				var innerDevice = new devType()
				{
					name = device.DeviceState.PlaceInTree
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
			ClientsCash.MonitoringFiresecManager.Watcher.ImitatorStateChanged(coreStates);
		}
	}
}