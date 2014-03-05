using System;
using FiresecAPI;
using Infrastructure.Common.Validation;
using SKDModule.Events;

namespace SKDModule.Validation
{
	public class WeeklyIntervalValidationError : ObjectValidationError<SKDWeeklyInterval, ShowSKDWeeklyIntervalsEvent, Guid>
	{
		public WeeklyIntervalValidationError(SKDWeeklyInterval interval, string error, ValidationErrorLevel level)
			: base(interval, error, level)
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