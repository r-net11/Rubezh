using System;
using AutomationModule.Events;
using RubezhAPI.Automation;
using Infrastructure.Common.Validation;
using RubezhAPI;
using Infrastructure.Common;

namespace AutomationModule.Validation
{
	class ProcedureValidationError : ObjectValidationError<Procedure, ShowProceduresEvent, Guid>
	{
		public ProcedureValidationError(Procedure procedure, string error, ValidationErrorLevel level)
			: base(procedure, error, level)
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