using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using XFiresecAPI;

namespace DeviceControls.XDevice
{
	public class XStateViewModel : BaseStateViewModel<LibraryXFrame>
	{
		public XStateViewModel(LibraryXState state, ICollection<Canvas> stateCanvases)
			: base(state.XFrames, stateCanvases)
		{
		}
	}
}