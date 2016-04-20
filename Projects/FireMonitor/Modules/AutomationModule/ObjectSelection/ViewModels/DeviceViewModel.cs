using RubezhAPI.GK;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.TreeList;

namespace AutomationModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public GKDevice Device { get; private set; }
		
		public DeviceViewModel(GKDevice device)
		{
			Device = device;
		}
	}
}