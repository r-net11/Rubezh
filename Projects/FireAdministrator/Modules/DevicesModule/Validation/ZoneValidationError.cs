using System;
using FiresecAPI.Models;
using Infrastructure.Common.Validation;
using Infrastructure.Events;

namespace DevicesModule.Validation
{
	class ZoneValidationError : ObjectValidationError<Zone, ShowZoneEvent, Guid>
	{
		public ZoneValidationError(Zone zone, string error, ValidationErrorLevel level)
			: base(zone, error, level)
		{
		}

		public override string Module
		{
			get { return "FS"; }
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
            get { return Object.No.ToString(); }
        }
        public override string ImageSource
        {
            get { return "/Controls;component/Images/Blue_Direction.png"; }
        }
	}
}