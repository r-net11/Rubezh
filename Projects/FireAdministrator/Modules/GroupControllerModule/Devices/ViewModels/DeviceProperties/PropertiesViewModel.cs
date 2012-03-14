using System.Collections.Generic;
using Infrastructure.Common;
using GroupControllerModule.Models;

namespace GroupControllerModule.ViewModels
{
    public class PropertiesViewModel : BaseViewModel
    {
        public XDevice XDevice { get; private set; }
        public List<StringPropertyViewModel> StringProperties { get; set; }
        public List<BoolPropertyViewModel> BoolProperties { get; set; }
        public List<EnumPropertyViewModel> EnumProperties { get; set; }

        public PropertiesViewModel(XDevice xDevice)
        {
            XDevice = xDevice;
            StringProperties = new List<StringPropertyViewModel>();
            BoolProperties = new List<BoolPropertyViewModel>();
            EnumProperties = new List<EnumPropertyViewModel>();

            foreach (var driverProperty in xDevice.Driver.Properties)
            {
                switch (driverProperty.DriverPropertyType)
                {
                    case XDriverPropertyTypeEnum.EnumType:
                        EnumProperties.Add(new EnumPropertyViewModel(driverProperty, xDevice));
                        break;

                    case XDriverPropertyTypeEnum.StringType:
                    case XDriverPropertyTypeEnum.IntType:
                    case XDriverPropertyTypeEnum.ByteType:
                        StringProperties.Add(new StringPropertyViewModel(driverProperty, xDevice));
                        break;

                    case XDriverPropertyTypeEnum.BoolType:
                        BoolProperties.Add(new BoolPropertyViewModel(driverProperty, xDevice));
                        break;
                }
            }
        }
    }
}