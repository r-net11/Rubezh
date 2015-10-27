using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.Processor
{
	public class ErrorsChangedEventArgs : EventArgs
	{
		public Guid DeviceUID { get; set; }
		public List<DeviceError> Errors { get; set; }

		public ErrorsChangedEventArgs()
		{
			Errors = new List<DeviceError>();
		}

		public ErrorsChangedEventArgs(ResursNetwork.OSI.ApplicationLayer.Devices.DeviceErrorOccuredEventArgs args)
			: this()
		{
			DeviceUID = args.Id;
			if (args.Errors.CommunicationError)
				Errors.Add(DeviceError.Communication);
			if (args.Errors.ConfigurationError)
				Errors.Add(DeviceError.Configuration);
			if (args.Errors.RTCError)
				Errors.Add(DeviceError.RTC);
		}

		public ErrorsChangedEventArgs(ResursNetwork.OSI.ApplicationLayer.NetworkControllerErrorOccuredEventArgs args)
			: this()
		{
			DeviceUID = args.Id;
			if (args.Errors.PortError)
				Errors.Add(DeviceError.Port);
		}
	}
}
