using AutomationModule.Events;
using FiresecAPI.Enums;
using FiresecAPI.Models.Automation;
using Infrastructure.Common.Validation;
using System;

namespace AutomationModule.Validation
{
	class VariableValidationError : ObjectValidationError<IVariable, ShowProceduresEvent, Guid>
	{
		public VariableValidationError(IVariable variable, string error, ValidationErrorLevel level)
			: base(variable, error, level)
		{
		}

		public override ModuleType Module
		{
			get { return ModuleType.Automation; }
		}
		protected override Guid Key
		{
			get { return Object.UID; }
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
			get { return "/Controls;component/Images/ProcedureYellow.png"; }
		}
	}
}
