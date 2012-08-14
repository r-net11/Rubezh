using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Validation;
using FiresecAPI.Models;
using Infrastructure.Events;

namespace DevicesModule.Validation
{
	class DeviceValidationError : ObjectValidationError<Device, ShowDeviceEvent, Guid>
	{
		public DeviceValidationError(Device device, string error, ValidationErrorLevel level)
			: base(device, error, level)
		{
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
