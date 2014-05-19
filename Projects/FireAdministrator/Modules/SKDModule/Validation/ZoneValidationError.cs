using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using SKDModule.Events;

namespace SKDModule.Validation
{
	public class ZoneValidationError : ObjectValidationError<SKDZone, ShowSKDZoneEvent, Guid>
	{
		public ZoneValidationError(SKDZone zone, string error, ValidationErrorLevel level)
			: base(zone, error, level)
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
			get { return "/Controls;component/Images/zone.png"; }
		}
	}
}