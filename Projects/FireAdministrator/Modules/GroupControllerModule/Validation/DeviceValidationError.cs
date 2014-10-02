using System;
using FiresecAPI.GK;
using Infrastructure.Common.Validation;
using Infrastructure.Events;

namespace GKModule.Validation
{
	public class DeviceValidationError : ObjectValidationError<GKDevice, ShowXDeviceEvent, Guid>
	{
		public DeviceValidationError(GKDevice device, string error, ValidationErrorLevel level)
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
		public override string ImageSource
		{
			get { return Object.Driver.ImageSource; }
		}
	}
}