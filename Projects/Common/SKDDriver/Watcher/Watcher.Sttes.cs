using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using System.Threading;
using Common;
using XFiresecAPI;
using FiresecAPI.XModels;

namespace SKDDriver
{
	public partial class Watcher
	{
		void GetAllStates()
		{
			GetState(Device);
			foreach (var childDevice in Device.Children)
			{
				GetState(childDevice);
			}
		}

		void GetState(SKDDevice device)
		{
			var bytes = new List<byte>();
			bytes.Add(4);
			var intAddress = -1;
			if (device.DriverType == SKDDriverType.Reader || device.DriverType == SKDDriverType.Gate)
			{
				try
				{
					intAddress = Int32.Parse(device.Address);
				}
				catch { }
			}
			bytes.Add((byte)intAddress);
			var sendResult = SKDDeviceProcessor.SendBytes(Device, bytes);
			if (!sendResult.HasError)
			{
				var stateClass = (XStateClass)sendResult.Bytes[0];
				device.State.StateClasses = new List<XStateClass>();
				device.State.StateClasses.Add(stateClass);
			}
		}
	}
}