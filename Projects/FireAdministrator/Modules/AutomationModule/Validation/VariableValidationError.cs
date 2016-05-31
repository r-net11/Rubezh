using System;
using AutomationModule.Events;
using StrazhAPI.Automation;
using StrazhAPI.Enums;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	class VariableValidationError : ObjectValidationError<Variable, ShowProceduresEvent, Guid>
	{
		public VariableValidationError(Variable variable, string error, ValidationErrorLevel level)
			: base(variable, error, level)
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
			get { return ""; }
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
