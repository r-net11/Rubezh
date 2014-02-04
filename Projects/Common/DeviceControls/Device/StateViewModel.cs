using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
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