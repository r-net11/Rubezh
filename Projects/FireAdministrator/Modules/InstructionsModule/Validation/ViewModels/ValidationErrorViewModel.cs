using FiresecClient.Validation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using FiresecAPI.Models;

namespace InstructionsModule.Validation.ViewModels
{
    public class ValidationErrorViewModel : BaseViewModel
    {
        InstructionError _instructionError;
        public ValidationErrorViewModel(InstructionError instructionError)
        {
            _instructionError = instructionError;
            Source = "";
            Address = instructionError.Instruction.No;
            Error = instructionError.Error;
        }

        public string Source { get; set; }
        public string Address { get; set; }
        public string Error { get; set; }

        public void Select()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_instructionError.Instruction.No);
        }
    }
}
