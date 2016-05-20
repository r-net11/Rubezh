using Infrastructure.Common.TreeList;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class SimpleDeviceViewModel : TreeNodeViewModel<SimpleDeviceViewModel>
	{
		public GKDevice Device { get; private set; }
		public SimpleDeviceViewModel(GKDevice device)
		{
			Device = device;
		}

	}
}