using Infrastructure.Common.Validation;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.Validation
{
	class ZoneValidationError : ObjectValidationError<XZone, ShowXZoneEvent, ushort>
	{
		public ZoneValidationError(XZone zone, string error, ValidationErrorLevel level)
			: base(zone, error, level)
		{
		}

		public override string Module
		{
			get { return "GK"; }
		}

		protected override ushort Key
		{
			get { return Object.No; }
		}

		public override string Source
		{
			get { return Object.Name; }
		}
	}
}