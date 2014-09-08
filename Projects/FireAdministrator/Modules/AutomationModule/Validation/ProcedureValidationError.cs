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
}