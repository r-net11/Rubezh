using System;
using FiresecAPI.Models;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using Infrastructure.Common;

namespace DevicesModule.Validation
{
	class DeviceValidationError : ObjectValidationError<Device, ShowDeviceEvent, Guid>
	{
		public DeviceValidationError(Device device, string error, ValidationErrorLevel level)
			: base(device, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.Devices; }
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
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Blue_Direction.png"; }
		}
	}
}