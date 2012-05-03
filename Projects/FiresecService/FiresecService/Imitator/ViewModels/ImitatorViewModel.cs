using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace FiresecService.ViewModels
{
	public class ImitatorViewModel : DialogContent
	{
		FiresecService.Service.FiresecService FiresecService;

		public ImitatorViewModel(FiresecService.Service.FiresecService firesecService)
		{
			FiresecService = firesecService;

			Title = "Имитатор устройств";
			Devices = new ObservableCollection<DeviceViewModel>();

			foreach (var deviceState in FiresecService.FiresecManager.DeviceConfigurationStates.DeviceStates)
			{
				var deviceViewModel = new DeviceViewModel(deviceState, FiresecService);
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
	}
}
