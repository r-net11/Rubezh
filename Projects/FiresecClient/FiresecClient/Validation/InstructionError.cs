using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecClient.Validation
{
    public class InstructionError : BaseError
    {
        public InstructionError(Instruction instruction, string error, ErrorLevel level)
            : base(error,level)
        {
            Instruction = instruction;
        }

        public Instruction Instruction { get; set; }
    }
}
