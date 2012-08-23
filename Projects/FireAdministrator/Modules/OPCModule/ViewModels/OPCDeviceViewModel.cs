using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;

namespace OPCModule.ViewModels
{
	public class OPCDeviceViewModel : TreeBaseViewModel<OPCDeviceViewModel>
	{
		public Device Device { get; private set; }

		public OPCDeviceViewModel(Device device, ObservableCollection<OPCDeviceViewModel> sourceDevices)
		{
			Device = device;
			Children = new ObservableCollection<OPCDeviceViewModel>();
			Source = sourceDevices;
		}

		public bool IsOPCUsed
		{
			get { return Device.IsOPCUsed; }
			set
			{
				Device.IsOPCUsed = value;
				OnPropertyChanged("IsOPCUsed");
                ServiceFactory.SaveService.OPCChanged = true;
                ServiceFactory.SaveService.DevicesChanged = true;
			}
		}
	}
}