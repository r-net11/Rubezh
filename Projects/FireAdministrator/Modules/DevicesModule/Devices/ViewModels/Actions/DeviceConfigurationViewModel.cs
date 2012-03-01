using System;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class DeviceConfigurationViewModel : DialogContent
    {
        Guid _deviceUID;
        DeviceConfiguration _deviceConfiguration;

        public DeviceConfigurationViewModel(Guid deviceUID, DeviceConfiguration deviceConfiguration)
        {
            Title = "Сравнение конфигураций";
            _deviceUID = deviceUID;
            _deviceConfiguration = deviceConfiguration;
            _deviceConfiguration.Update();
        }
    }
}