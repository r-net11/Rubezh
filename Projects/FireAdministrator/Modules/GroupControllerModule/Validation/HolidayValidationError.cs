using System;
using FiresecAPI.GK;
using GKModule.Events;
using Infrastructure.Common.Validation;
using Infrastructure.Common;

namespace GKModule.Validation
{
	class HolidayValidationError : ObjectValidationError<GKHoliday, ShowGKHolidayEvent, Guid>
	{
		public HolidayValidationError(GKHoliday holiday, string error, ValidationErrorLevel level)
			: base(holiday, error, level)
		{
		}

        public override ModuleType Module
		{
			get { return ModuleType.GK; }
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
			get { return "/Controls;component/Images/Holiday.png"; }
		}
	}
}