using System;
using GKModule.Events;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public class GuardUserValidationError : ObjectValidationError<XGuardUser, ShowXGuardEvent, Guid>
	{
		public GuardUserValidationError(XGuardUser guardUser, string error, ValidationErrorLevel level)
			: base(guardUser, error, level)
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
			get { return Object.Name; }
		}
		public override string ImageSource
		{
			get { return "/Controls;component/Images/BUser.png"; }
		}
	}
}