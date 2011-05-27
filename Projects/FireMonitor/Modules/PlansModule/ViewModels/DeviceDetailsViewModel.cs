using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using DeviceLibrary.Models;
using FiresecClient;

namespace PlansModule.ViewModels
{
    public class DeviceDetailsViewModel : DialogContent
    {
        public DeviceDetailsViewModel()
        {
            Title = "Свойства устройства";
        }

        FiresecClient.Device _device;
        Firesec.Metadata.drvType _driver;

        public void Initialize(FiresecClient.Device device)
        {
            _device = device;
            _driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
        }

        public string DeviceCategory
        {
            get
            {
                switch (_driver.cat)
                {
                    case "0":
                        return "Прочие устройства";

                    case "1":
                        return "Приборы";

                    case "2":
                        return "Датчики";

                    case "3":
                        return "ИУ";

                    case "4":
                        return "Сеть передачи данных";

                    case "5":
                        return "Не указано";

                    case "6":
                        return "Удаленный сервер";
                }

                return "";
            }
        }
    }
}
