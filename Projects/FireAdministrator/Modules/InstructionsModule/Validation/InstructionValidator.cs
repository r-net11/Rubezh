using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Validation;
using FiresecClient;

namespace InstructionsModule.Validation
{
	public static class InstructionValidator
	{
		public static IEnumerable<IValidationError> Validate()
		{
			var errors = new List<IValidationError>();

			foreach (var instruction in FiresecManager.SystemConfiguration.Instructions)
				if (FiresecManager.SystemConfiguration.Instructions.Count(x => x.No == instruction.No) > 1)
					yield return  new InstructionValidationError(instruction, "Инструкция с таким номером уже существует!", ValidationErrorLevel.Warning);

			foreach (var instruction in FiresecManager.SystemConfiguration.Instructions)
				if (FiresecManager.SystemConfiguration.Instructions.Count(x =>					((x.StateType == instruction.StateType) && (x.InstructionType == InstructionType.General))) > 1)
					yield return new InstructionValidationError(instruction, "Общая инструкция для данного состояния уже существует!", ValidationErrorLevel.Warning);
		}
	}
}