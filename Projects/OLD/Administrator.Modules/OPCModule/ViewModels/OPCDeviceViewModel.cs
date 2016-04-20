using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.TreeList;

namespace OPCModule.ViewModels
{
	public class OPCDeviceViewModel : TreeNodeViewModel<OPCDeviceViewModel>
	{
		public Device Device { get; private set; }

		public OPCDeviceViewModel(Device device)
		{
			Device = device;
		}

		public bool CanOPCUsed
		{
			get { return Device.Driver.IsPlaceable; }
		}

		public bool IsOPCUsed
		{
			get { return Device.IsOPCUsed; }
			set
			{
				Device.IsOPCUsed = value;
				OnPropertyChanged(() => IsOPCUsed);
				ServiceFactory.SaveService.OPCChanged = true;
				ServiceFactory.SaveService.FSChanged = true;
			}
		}
	}
}