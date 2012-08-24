using FiresecAPI.Models;
using Infrastructure.Common.Validation;
using Infrastructure.Events;

namespace InstructionsModule.Validation
{
	class InstructionValidationError : ObjectValidationError<Instruction, ShowInstructionsEvent, int?>
	{
		public InstructionValidationError(Instruction instruction, string error, ValidationErrorLevel level)
			: base(instruction, error, level)
		{
		}

		public override string Module
		{
			get { return "FS"; }
		}

		public override string Source
		{
			get { return "Инструкция"; }
		}

		protected override int? Key
		{
			get { return Object.No; }
		}
	}
}
