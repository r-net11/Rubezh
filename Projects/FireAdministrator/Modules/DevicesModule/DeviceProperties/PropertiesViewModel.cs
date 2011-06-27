using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using FiresecClient.Models;

namespace DevicesModule.DeviceProperties
{
    public class PropertiesViewModel : BaseViewModel
    {
        public List<StringPropertyViewModel> StringProperties { get; set; }
        public List<BoolPropertyViewModel> BoolProperties { get; set; }
        public List<EnumPropertyViewModel> EnumProperties { get; set; }

        public PropertiesViewModel(Device device)
        {
            StringProperties = new List<StringPropertyViewModel>();
            BoolProperties = new List<BoolPropertyViewModel>();
            EnumProperties = new List<EnumPropertyViewModel>();

            foreach (var driverProperty in device.Driver.Properties)
            {
                switch (driverProperty.DriverPropertyType)
                {
                    case DriverProperty.DriverPropertyTypeEnum.EnumType:
                        EnumProperties.Add(new EnumPropertyViewModel(driverProperty, device));
                        break;
                    case DriverProperty.DriverPropertyTypeEnum.StringType:
                    case DriverProperty.DriverPropertyTypeEnum.IntType:
                    case DriverProperty.DriverPropertyTypeEnum.ByteType:
                        StringProperties.Add(new StringPropertyViewModel(driverProperty, device));
                        break;
                    case DriverProperty.DriverPropertyTypeEnum.BoolType:
                        BoolProperties.Add(new BoolPropertyViewModel(driverProperty, device));
                        break;
                }
            }
        }
    }
}
