using System;
using StrazhAPI.Enums;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;
using StrazhModule.Events;
using Infrastructure.Common;

namespace StrazhModule.Validation
{
	public class SlideWeeklyIntervalValidationError : ObjectValidationError<SKDSlideWeeklyInterval, ShowSKDSlideWeekIntervalsEvent, int>
	{
		public SlideWeeklyIntervalValidationError(SKDSlideWeeklyInterval interval, string error, ValidationErrorLevel level)
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