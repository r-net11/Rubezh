using System;
using FiresecAPI.GK;
using Infrastructure.Common.Validation;
using GKModule.Events;

namespace GKModule.Validation
{
	public class DayScheduleValidationError : ObjectValidationError<GKDaySchedule, ShowGKDaySchedulesEvent, Guid>
	{
		public DayScheduleValidationError(GKDaySchedule daySchedule, string error, ValidationErrorLevel level)
			: base(daySchedule, error, level)
		{
		}

		public override string Module
		{
			get { return "GK"; }
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
			get { return "/Controls;component/Images/Shedule.png"; }
		}
	}
}