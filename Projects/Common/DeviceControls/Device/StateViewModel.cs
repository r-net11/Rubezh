using System.Collections.Generic;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecAPI;

namespace DeviceControls.Device
{
	public class StateViewModel : BaseStateViewModel<LibraryFrame, StateType>
	{
		public StateViewModel(LibraryState state, ICollection<Canvas> stateCanvases)
			: base(state.Frames, stateCanvases)
		{
		}
	}
}