using FiresecAPI.Models;
using Infrastructure.Common.Validation;
using Infrastructure.Events;

namespace DevicesModule.Validation
{
	class ZoneValidationError : ObjectValidationError<Zone, ShowZoneEvent, int>
	{
		public ZoneValidationError(Zone zone, string error, ValidationErrorLevel level)
			: base(zone, error, level)
		{
		}

		protected override int Key
		{
			get { return Object.No; }
		}

		public override string Source
		{
			get { return Object.Name; }
		}
	}
}
