using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using XFiresecAPI;

namespace DeviceControls.SKDDevice
{
	public class SKDStateViewModel : BaseStateViewModel<SKDLibraryFrame>
	{
		public SKDStateViewModel(SKDLibraryState state, ICollection<Canvas> stateCanvases)
			: base(state.Frames, stateCanvases)
		{
		}
	}
}