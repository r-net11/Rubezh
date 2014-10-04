using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using SKDModule.Events;

namespace SKDModule.Validation
{
	public class DayIntervalValidationError : ObjectValidationError<SKDDayInterval, ShowSKDDayIntervalsEvent, int>
	{
		public DayIntervalValidationError(SKDDayInterval interval, string error, ValidationErrorLevel level)
			: base(interval, error, level)
		{
		}

		public override string Module
		{
			get { return "SKD"; }
		}
		protected override int Key
		{
			get { return Object.No; }
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
			get { return "/Controls;component/Images/Shedule.png"; }
		}
	}
}