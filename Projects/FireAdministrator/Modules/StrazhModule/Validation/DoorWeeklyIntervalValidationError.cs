using System;
using FiresecAPI.Enums;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using StrazhModule.Events;
using Infrastructure.Common;

namespace StrazhModule.Validation
{
	public class DoorWeeklyIntervalValidationError : ObjectValidationError<SKDDoorWeeklyInterval, ShowSKDDoorWeeklyIntervalsEvent, int>
	{
		public DoorWeeklyIntervalValidationError(SKDDoorWeeklyInterval interval, string error, ValidationErrorLevel level)
			: base(interval, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.SKD; }
		}
		protected override int Key
		{
			get { return Object.ID; }
		}
		public override string Source
		{
			get { return Object.Name; }
		}
		public override string Address
		{
			get { return Object.ID.ToString(); }
		}
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Shedule.png"; }
		}
	}
}