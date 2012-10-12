using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace InstructionsModule.Validation
{
	public static class Validator
	{
		public static IEnumerable<IValidationError> Validate()
		{
			foreach (var instruction in FiresecManager.SystemConfiguration.Instructions)
				if (FiresecManager.SystemConfiguration.Instructions.Count(x => ((x.StateType == instruction.StateType) && (x.InstructionType == InstructionType.General))) > 1)
					yield return new InstructionValidationError(instruction, "Общая инструкция для данного состояния уже существует!", ValidationErrorLevel.Warning);
		}
	}
}