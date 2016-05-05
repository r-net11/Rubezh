using System;
using StrazhAPI.Enums;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;
using StrazhModule.Events;
using Infrastructure.Common;

namespace StrazhModule.Validation
{
	public class DeviceValidationError : ObjectValidationError<SKDDevice, ShowSKDDeviceEvent, Guid>
	{
		public DeviceValidationError(SKDDevice device, string error, ValidationErrorLevel level)
			: base(device, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.SKD; }
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
			get { return Object.Address; }
		}
		public override string ImageSource
		{
			get { return Object.Driver.ImageSource; }
		}
	}
}