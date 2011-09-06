using FiresecClient.Validation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
    public class ValidationErrorViewModel : BaseViewModel
    {
        ErrorEntityType _errorEntityType;
        ZoneError _zoneError;
        DeviceError _deviceError;
        InstructionError _instructionError;

        public ValidationErrorViewModel(DeviceError deviceError)
        {
            _errorEntityType = ErrorEntityType.Device;
            _deviceError = deviceError;
            Source = deviceError.Device.Driver.ShortName;
            Address = deviceError.Device.PresentationAddress;
            Error = deviceError.Error;
        }

        public ValidationErrorViewModel(ZoneError zoneError)
        {
            _errorEntityType = ErrorEntityType.Zone;
            _zoneError = zoneError;
            Source = zoneError.Zone.Name;
            Address = zoneError.Zone.No;
            Error = zoneError.Error;
        }

        public ValidationErrorViewModel(InstructionError instructionError)
        {
            _errorEntityType = ErrorEntityType.Instruction;
            _instructionError = instructionError;
            Source = "Инструкция";
            Address = instructionError.Instruction.No;
            Error = instructionError.Error;
        }

        public string Source { get; set; }
        public string Address { get; set; }
        public string Error { get; set; }

        public void Select()
        {
            switch (_errorEntityType)
            {
                case ErrorEntityType.Device:
                    ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_deviceError.Device.UID);
                    break;

                case ErrorEntityType.Zone:
                    ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(_zoneError.Zone.No);
                    break;

                case ErrorEntityType.Instruction:
                    ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Publish(_instructionError.Instruction.No);
                    break;
            }
        }
    }
}