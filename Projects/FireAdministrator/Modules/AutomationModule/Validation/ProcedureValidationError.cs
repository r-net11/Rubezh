using System;
using AutomationModule.Events;
using FiresecAPI.Automation;
using Infrastructure.Common.Validation;
using FiresecAPI;

namespace AutomationModule.Validation
{
	class ProcedureValidationError : ObjectValidationError<Procedure, ShowProceduresEvent, Guid>
	{
		public ProcedureValidationError(Procedure procedure, string error, ValidationErrorLevel level)
			: base(procedure, error, level)
		{
		}

		public override string Module
		{
			get { return "AutomationModule"; }
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

	class VariableValidationError : ObjectValidationError<Variable, ShowProceduresEvent, Guid>
	{
		public VariableValidationError(Variable variable, string error, ValidationErrorLevel level)
			: base(variable, error, level)
		{
		}

		public override string Module
		{
			get { return "AutomationModule"; }
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

	class ProcedureStepValidationError : ObjectValidationError<ProcedureStep, ShowProceduresEvent, Guid>
	{
		public ProcedureStepValidationError(ProcedureStep procedureStep, string error, ValidationErrorLevel level)
			: base(procedureStep, error, level)
		{
		}

		public override string Module
		{
			get { return "AutomationModule"; }
		}
		protected override Guid Key
		{
			get { return Object.UID; }
		}
		public override string Address
		{
			get { return ""; }
		}
		public override string Source
		{
			get { return Object.ProcedureStepType.ToDescription(); }
		}
		public override string ImageSource
		{
			get { return "/Controls;component/Images/ProcedureYellow.png"; }
		}
	}
}