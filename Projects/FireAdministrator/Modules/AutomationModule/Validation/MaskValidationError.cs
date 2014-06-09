using System;
using AutomationModule.Events;
using FiresecAPI.Automation;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	class MaskValidationError : ObjectValidationError<Mask, ShowMasksEvent, Guid>
	{
		public MaskValidationError(Mask mask, string error, ValidationErrorLevel level)
			: base(mask, error, level)
		{
		}

		public override string Module
		{
			get { return "Mask"; }
		}
		protected override Guid Key
		{
			get { return Object.Uid; }
		}
		public override string Address
		{
			get { return ""; }
		}
		public override string Source
		{
			get { return Object.Name; }
		}
		public override string ImageSource
		{
			get { return "/Controls;component/Images/SelectNone.png"; }
		}
	}
}