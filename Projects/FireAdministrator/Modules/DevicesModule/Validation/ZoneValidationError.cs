using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Validation;
using FiresecAPI.Models;
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
