using System;
using AutomationModule.Events;
using StrazhAPI.Automation;
using StrazhAPI.Enums;
using Infrastructure.Common.Validation;
using StrazhAPI;
using Infrastructure.Common;

namespace AutomationModule.Validation
{
	class ProcedureStepValidationError : ObjectValidationError<ProcedureStep, ShowProceduresEvent, Guid>
	{
		public ProcedureStepValidationError(ProcedureStep procedureStep, string error, ValidationErrorLevel level)
			: base(procedureStep, error, level)
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
