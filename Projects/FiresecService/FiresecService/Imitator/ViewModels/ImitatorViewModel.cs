using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace FiresecService.ViewModels
{
	public class ImitatorViewModel : DialogContent
	{
		FiresecManager FiresecManager;

		public ImitatorViewModel(FiresecManager firesecManager)
		{
			FiresecManager = firesecManager;

			Title = "Имитатор устройств";
			Devices = new ObservableCollection<DeviceViewModel>();

			foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
			{
				var deviceViewModel = new DeviceViewModel(deviceState, FiresecManager);
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
