﻿using System;
using FiresecAPI.Enums;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using StrazhModule.Events;
using Infrastructure.Common;

namespace StrazhModule.Validation
{
	public class DoorDayIntervalValidationError : ObjectValidationError<SKDDoorDayInterval, ShowSKDDoorDayIntervalsEvent, Guid>
	{
		public DoorDayIntervalValidationError(SKDDoorDayInterval interval, string error, ValidationErrorLevel level)
			: base(interval, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.SKD; }
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