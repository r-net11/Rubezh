using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecAPI.SKD;

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
			bytes.Add((byte)device.IntAddress);
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