using System;
using FiresecAPI.GK;
using GKModule.Events;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public class CodeValidationError : ObjectValidationError<GKCode, ShowXGuardEvent, Guid>
	{
		public CodeValidationError(GKCode code, string error, ValidationErrorLevel level)
			: base(code, error, level)
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