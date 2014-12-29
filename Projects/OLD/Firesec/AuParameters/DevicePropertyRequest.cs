using System;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace Firesec_50
{
	public class DevicePropertyRequest
	{
		public DevicePropertyRequest(Device device)
		{
			Device = device;
			Properties = new List<Property>();
			PropertyNos = new HashSet<int>();
			RequestIds = new List<RequestInfo>();

			foreach (var property in device.Driver.Properties)
			{
				if (property.IsAUParameter)
				{
					PropertyNos.Add(property.No);
				}
			}
		}

		public Device Device { get; set; }
		public List<Property> Properties { get; set; }
		public HashSet<int> PropertyNos { get; set; }
		public List<RequestInfo> RequestIds { get; set; }

		DateTime StartDateTime = DateTime.Now;
		public bool IsDeleting
		{
			get
			{
				var timeoutExpired = DateTime.Now - StartDateTime > TimeSpan.FromMinutes(5) || RequestIds.Count == 0;
				if (timeoutExpired)
				{
					Device.OnAUParametersChanged();
				}
				return timeoutExpired;
			}
		}

		public class RequestInfo
		{
			public int ParamNo { get; set; }
			public int RequestId { get; set; }
		}
	}
}