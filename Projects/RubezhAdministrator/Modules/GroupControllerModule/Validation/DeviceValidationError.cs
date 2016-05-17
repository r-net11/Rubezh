using System;
using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Validation;
using Infrastructure.Events;

namespace GKModule.Validation
{
	public class DeviceValidationError : ObjectValidationError<GKDevice, ShowGKDeviceEvent, Guid>
	{
		public DeviceValidationError(GKDevice device, string error, ValidationErrorLevel level)
			: base(device, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.GK; }
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
			get { return Object.Driver.ImageSource; }
		}
	}
}