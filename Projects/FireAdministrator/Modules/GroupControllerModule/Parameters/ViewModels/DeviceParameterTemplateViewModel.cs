using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DeviceParameterTemplateViewModel : BaseViewModel
	{
		public GKDeviceParameterTemplate DeviceParameterTemplate { get; private set; }
		public DeviceParameterViewModel DeviceParameterViewModel { get; private set; }

		public DeviceParameterTemplateViewModel(GKDeviceParameterTemplate deviceParameterTemplate)
		{
			DeviceParameterTemplate = deviceParameterTemplate;
			DeviceParameterViewModel = new DeviceParameterViewModel(deviceParameterTemplate.GKDevice);
		}

		public GKDriver Driver
		{
			get { return DeviceParameterTemplate.GKDevice.Driver; }
		}
	}
}