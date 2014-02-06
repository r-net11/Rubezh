using System.Collections.Generic;
using System.Windows.Controls;
using XFiresecAPI;

namespace DeviceControls.XDevice
{
	public class XStateViewModel : BaseStateViewModel<LibraryXFrame, XStateClass>
	{
		public XStateViewModel(LibraryXState state, ICollection<Canvas> stateCanvases)
			: base(state.XFrames, stateCanvases)
		{
		}
	}
}