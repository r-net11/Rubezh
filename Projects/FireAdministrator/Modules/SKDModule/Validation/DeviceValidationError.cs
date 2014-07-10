using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using SKDModule.Events;

namespace SKDModule.Validation
{
	public class DeviceValidationError : ObjectValidationError<SKDDevice, ShowSKDDeviceEvent, Guid>
	{
		public DeviceValidationError(SKDDevice device, string error, ValidationErrorLevel level)
			: base(device, error, level)
		{
		}

		public override string Module
		{
			get { return "SKD"; }
		}
		protected override Guid Key
		{
			get { return Object.UID; }
		}
		public override string Source
		{
			get { return Object.Name; }
		}
		public override string Address
		{
			get { return ""; }
		}
		public override string ImageSource
		{
			get { return Object.Driver.ImageSource; }
		}
	}
}