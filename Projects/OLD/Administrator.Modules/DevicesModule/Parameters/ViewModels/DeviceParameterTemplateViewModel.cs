using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceParameterTemplateViewModel : BaseViewModel
	{
		public DeviceParameterTemplate DeviceParameterTemplate { get; private set; }
		public DeviceParameterViewModel DeviceParameterViewModel { get; private set; }

		public DeviceParameterTemplateViewModel(DeviceParameterTemplate deviceParameterTemplate)
		{
			DeviceParameterTemplate = deviceParameterTemplate;
			DeviceParameterViewModel = new DeviceParameterViewModel(deviceParameterTemplate.Device);
		}

		public Driver Driver
		{
			get { return DeviceParameterTemplate.Device.Driver; }
		}
	}
}