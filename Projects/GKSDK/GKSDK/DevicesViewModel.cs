using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace GKSDK
{
	public class DevicesViewModel : BaseViewModel
	{
		public DevicesViewModel()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in GKManager.Devices.Where(x => x.IsRealDevice))
			{
				var deviceViewModel = new DeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
				OnPropertyChanged("StateType");
			}
		}
		public bool IsBiStateControl
		{
			get 
			{ 
				if (SelectedDevice!=null)
				return SelectedDevice.Device.Driver.IsDeviceOnShleif && !SelectedDevice.Device.Driver.IsControlDevice && FiresecManager.CheckPermission(PermissionType.Oper_Device_Control);
				return true;
			}
		}
	}
}