using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient.Validation;
using Infrastructure;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
    public class ValidationErrorViewModel : BaseViewModel
    {
        ErrorEntityType _errorEntityType;
        ZoneError _zoneError;
        DeviceError _deviceError;

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

        public string Source { get; set; }
        public string Address { get; set; }
        public string Error { get; set; }

        public void Select()
        {
            switch (_errorEntityType)
            {
                case ErrorEntityType.Device:
                    ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_deviceError.Device.Id);
                    break;

                case ErrorEntityType.Zone:
                    ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(_zoneError.Zone.No);
                    break;
            }
        }
    }

    enum ErrorEntityType
    {
        Device,
        Zone
    }
}
