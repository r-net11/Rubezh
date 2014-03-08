using System;
using FiresecAPI;
using Infrastructure.Common.Validation;
using SKDModule.Events;

namespace SKDModule.Validation
{
	public class HolidayValidationError : ObjectValidationError<SKDHoliday, ShowSKDHolidaysEvent, Guid>
	{
		public HolidayValidationError(SKDHoliday holiday, string error, ValidationErrorLevel level)
			: base(holiday, error, level)
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