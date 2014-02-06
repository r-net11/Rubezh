using System.Collections.Generic;
using System.Windows.Controls;
using XFiresecAPI;

namespace DeviceControls.SKDDevice
{
	public class SKDStateViewModel : BaseStateViewModel<SKDLibraryFrame, XStateClass>
	{
		public SKDStateViewModel(SKDLibraryState state, ICollection<Canvas> stateCanvases)
			: base(state.Frames, stateCanvases)
		{
		}
	}
}