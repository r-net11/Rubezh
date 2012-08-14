using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Validation;
using FiresecAPI.Models;
using Infrastructure.Events;

namespace DevicesModule.Validation
{
	class DirectionValidationError : ObjectValidationError<Direction, ShowDirectionsEvent, int?>
	{
		public DirectionValidationError(Direction direction, string error, ValidationErrorLevel level)
			: base(direction, error, level)
		{
		}

		protected override int? Key
		{
			get { return Object.Id; }
		}

		public override string Source
		{
			get { return Object.Name; }
		}
	}
}
