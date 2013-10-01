using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ZoneDeviceViewModel : TreeNodeViewModel<ZoneDeviceViewModel>
	{
		public XDevice Device { get; private set; }

		public ZoneDeviceViewModel(XDevice device)
		{
			Device = device;
		}

		public XDriver Driver
		{
			get { return Device.Driver; }
		}
		public string PresentationAddress
		{
			get { return Device.PresentationAddress; }
		}
		public string Description
		{
			get { return Device.Description; }
		}
		public bool IsBold { get; set; }
	}
}