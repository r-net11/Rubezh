using System;
using FiresecAPI.GK;
using GKModule.Events;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public class DelayValidationError : ObjectValidationError<GKDelay, ShowXDelayEvent, Guid>
	{
		public DelayValidationError(GKDelay delay, string error, ValidationErrorLevel level)
			: base(delay, error, level)
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
			get { return Object.No.ToString(); }
		}

		public override string ImageSource
		{
			get { return "/Controls;component/Images/Delay.png"; }
		}
	}
}