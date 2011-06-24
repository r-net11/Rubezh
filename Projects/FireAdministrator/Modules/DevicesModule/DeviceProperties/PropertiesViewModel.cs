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

            var driver = FiresecManager.Configuration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);

            if (driver.propInfo != null)
            {
                foreach (var propertyInfo in driver.propInfo)
                {
                    if (propertyInfo.hidden == "1")
                        continue;
                    if ((propertyInfo.caption == "Заводской номер") || (propertyInfo.caption == "Версия микропрограммы"))
                        continue;

                    if (propertyInfo.param != null)
                    {
                        EnumProperties.Add(new EnumPropertyViewModel(propertyInfo, device));
                    }
                    else
                    {
                        switch (propertyInfo.type)
                        {
                            case "String":
                            case "Int":
                            case "Byte":
                                StringProperties.Add(new StringPropertyViewModel(propertyInfo, device));
                                break;
                            case "Bool":
                                BoolProperties.Add(new BoolPropertyViewModel(propertyInfo, device));
                                break;
                            default:
                                throw new Exception("Неизвестный тип свойства");
                        }
                    }
                }
            }
        }
    }
}
