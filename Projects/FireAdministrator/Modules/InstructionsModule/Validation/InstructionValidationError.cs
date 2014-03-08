using System;
using FiresecAPI.Models;
using Infrastructure.Common.Validation;
using Infrastructure.Events;

namespace InstructionsModule.Validation
{
	class InstructionValidationError : ObjectValidationError<Instruction, ShowInstructionsEvent, Guid>
	{
		public InstructionValidationError(Instruction instruction, string error, ValidationErrorLevel level)
			: base(instruction, error, level)
		{
		}

		public override string Module
		{
			get { return "FS"; }
		}
		protected override Guid Key
		{
			get { return Object.UID; }
		}
		public override string Source
		{
			get { return Object.Name; }
		}
		public override string Address
		{
			get { return ""; }
		}
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Blue_Direction.png"; }
		}
	}
}