using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.Processor
{
	public class IsActiveChangedEventArgs : EventArgs
	{
		public Guid DeviceUID { get; set; }
		public bool IsActive { get; set; }

		public IsActiveChangedEventArgs() { }

		public IsActiveChangedEventArgs(ResursNetwork.Networks.StatusChangedEventArgs args)
		{
			DeviceUID = args.Id;
			IsActive = args.Status == ResursNetwork.Management.Status.Running;
		}
	}
}
