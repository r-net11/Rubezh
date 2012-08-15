using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Validation;
using FiresecAPI.Models;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.Validation
{
	class ZoneValidationError : ObjectValidationError<XZone, ShowXZonesEvent, object>
	{
		public ZoneValidationError(XZone zone, string error, ValidationErrorLevel level)
			: base(zone, error, level)
		{
		}

		protected override object Key
		{
			get { return Object.No; }
		}

		public override string Source
		{
			get { return Object.Name; }
		}
	}
}