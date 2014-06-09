using System;
using AutomationModule.Events;
using FiresecAPI.Automation;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	class GlobalVariableValidationError : ObjectValidationError<GlobalVariable, ShowGlobalVariablesEvent, Guid>
	{
		public GlobalVariableValidationError(GlobalVariable globalVariable, string error, ValidationErrorLevel level)
			: base(globalVariable, error, level)
		{
		}

		public override string Module
		{
			get { return "GlobalVariable"; }
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