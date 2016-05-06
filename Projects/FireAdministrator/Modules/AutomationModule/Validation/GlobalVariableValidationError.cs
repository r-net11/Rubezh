using AutomationModule.Events;
using StrazhAPI.Enums;
using StrazhAPI.Models.Automation;
using Infrastructure.Common.Validation;
using System;

namespace AutomationModule.Validation
{
	class GlobalVariableValidationError : ObjectValidationError<IVariable, ShowGlobalVariablesEvent, Guid>
	{
		public GlobalVariableValidationError(IVariable globalVariable, string error, ValidationErrorLevel level)
			: base(globalVariable, error, level)
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
			get { return "/Controls;component/Images/SelectNone.png"; }
		}
	}
}