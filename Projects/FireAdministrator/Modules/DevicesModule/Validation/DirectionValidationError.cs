using System;
using FiresecAPI.Models;
using Infrastructure.Common.Validation;
using Infrastructure.Events;

namespace DevicesModule.Validation
{
	class DirectionValidationError : ObjectValidationError<Direction, ShowDirectionsEvent, Guid>
	{
		public DirectionValidationError(Direction direction, string error, ValidationErrorLevel level)
			: base(direction, error, level)
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
	}
}