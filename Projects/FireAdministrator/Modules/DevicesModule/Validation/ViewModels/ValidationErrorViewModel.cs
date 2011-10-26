using FiresecClient.Validation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
    public class ValidationErrorViewModel : BaseViewModel
    {
        ErrorEntityType _errorEntityType;

        DeviceError _deviceError;
        ZoneError _zoneError;
        DirectionError _directionError;
        InstructionError _instructionError;

        public ValidationErrorViewModel(DeviceError deviceError)
        {
            _errorEntityType = ErrorEntityType.Device;
            _deviceError = deviceError;

            if (deviceError.Device != null)
            {
                Source = deviceError.Device.Driver.ShortName;
                Address = deviceError.Device.DottedAddress;
            }
            Error = deviceError.Error;
            ErrorLevel = deviceError.Level;
        }

        public ValidationErrorViewModel(ZoneError zoneError)
        {
            _errorEntityType = ErrorEntityType.Zone;
            _zoneError = zoneError;

            Source = zoneError.Zone.Name;
            Address = zoneError.Zone.No.ToString();
            Error = zoneError.Error;
            ErrorLevel = zoneError.Level;
        }

        public ValidationErrorViewModel(DirectionError directionError)
        {
            _errorEntityType = ErrorEntityType.Direction;
            _directionError = directionError;

            Source = directionError.Direction.Name;
            Address = directionError.Direction.Id.ToString();
            Error = directionError.Error;
            ErrorLevel = directionError.Level;
        }

        public ValidationErrorViewModel(InstructionError instructionError)
        {
            _errorEntityType = ErrorEntityType.Instruction;
            _instructionError = instructionError;

            Source = "Инструкция";
            Address = instructionError.Instruction.No.ToString();
            Error = instructionError.Error;
            ErrorLevel = instructionError.Level;
        }

        public string Source { get; private set; }
        public string Address { get; private set; }
        public string Error { get; private set; }
        public ErrorLevel ErrorLevel { get; private set; }

        public void Select()
        {
            switch (_errorEntityType)
            {
                case ErrorEntityType.Device:
                    if (_deviceError.Device != null)
                        ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_deviceError.Device.UID);
                    break;

                case ErrorEntityType.Zone:
                    ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(_zoneError.Zone.No);
                    break;

                case ErrorEntityType.Direction:
                    ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Publish(_directionError.Direction.Id);
                    break;

                case ErrorEntityType.Instruction:
                    ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Publish(_instructionError.Instruction.No);
                    break;
            }
        }
    }
}