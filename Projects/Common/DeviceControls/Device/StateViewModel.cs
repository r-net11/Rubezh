using System.Collections.Generic;
using System.Windows.Controls;
using FiresecAPI.Models;

namespace DeviceControls.Device
{
	public class StateViewModel : BaseStateViewModel<LibraryFrame>
	{
		public StateViewModel(LibraryState state, ICollection<Canvas> stateCanvases)
			: base(state.Frames, stateCanvases)
		{
		}
	}
}