using System;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.Validation
{
	public class DeviceValidationError : ObjectValidationError<XDevice, ShowXDeviceEvent, Guid>
	{
		public DeviceValidationError(XDevice device, string error, ValidationErrorLevel level)
			: base(device, error, level)
		{
		}

		public override string Module
		{
			get { return "GK"; }
		}

		protected override Guid Key
		{
			get { return Object.UID; }
		}

		public override string Source
		{
			get { return Object.Driver.ShortName; }
		}
		public override string Address
		{
			get { return Object.DottedAddress; }
		}
	}
}