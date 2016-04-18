using System;
using AutomationModule.Events;
using FiresecAPI.Automation;
using FiresecAPI.Enums;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	class GlobalVariableValidationError : ObjectValidationError<Variable, ShowGlobalVariablesEvent, Guid>
	{
		public GlobalVariableValidationError(Variable globalVariable, string error, ValidationErrorLevel level)
			: base(globalVariable, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.Automation; }
		}
		protected override Guid Key
		{
			get { return Object.Uid; }
		}
		public override string Address
		{
			get { return string.Empty; }
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