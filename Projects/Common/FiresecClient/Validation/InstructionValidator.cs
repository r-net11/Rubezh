using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient.Validation
{
    public static class InstructionValidator
    {
        public static List<InstructionError> InstructionErrors;

        public static void Validate()
        {
            InstructionErrors = new List<InstructionError>();

            foreach (var instruction in FiresecManager.SystemConfiguration.Instructions)
            {
                if (FiresecManager.SystemConfiguration.Instructions.Count(x => x.No == instruction.No) > 1)
                {
                    var instructionError =
                        new InstructionError(instruction, "Инструкция с таким номером уже существует!", ErrorLevel.Warning);
                    InstructionErrors.Add(instructionError);
                }
            }

            foreach (var instruction in FiresecManager.SystemConfiguration.Instructions)
            {
                if (FiresecManager.SystemConfiguration.Instructions.Count(x =>
                    ((x.StateType == instruction.StateType) && (x.InstructionType == InstructionType.General))) > 1)
                {
                    var instructionError =
                        new InstructionError(instruction, "Общая инструкция для данного состояния уже существует!", ErrorLevel.Warning);
                    InstructionErrors.Add(instructionError);
                }
            }

            //foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
            //{
            //    var instructionsZone = (from instruction in FiresecManager.SystemConfiguration.Instructions
            //                            where ((instruction.InstructionType == InstructionType.Zone) &&
            //                            (instruction.StateType == stateType))
            //                            select instruction);

            //}
        }
    }
}