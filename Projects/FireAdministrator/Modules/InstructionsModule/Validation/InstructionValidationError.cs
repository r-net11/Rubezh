using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Events;
using Infrastructure.Common.Validation;

namespace InstructionsModule.Validation
{
	class InstructionValidationError : ObjectValidationError<Instruction, ShowInstructionsEvent, int?>
	{
		public InstructionValidationError(Instruction instruction, string error, ValidationErrorLevel level)
			: base(instruction, error, level)
		{
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
