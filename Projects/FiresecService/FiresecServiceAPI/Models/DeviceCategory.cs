using System;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
    public enum DeviceCategoryType
    {
        Other,
        Device,
        Sensor,
        Effector,
        Communication,
        RemoteServer,
        None
    }

    [Serializable]
    public class DeviceCategory
    {
        [XmlAttribute]
        public int Id { get; set; }

        public DeviceCategoryType DeviceCategoryType
        {
            get { return (DeviceCategoryType) Id; }
        }

        public string DeviceCategoryName
        {
            get
            {
                switch (Id)
                {
                    case 0:
                        return "Прочие устройства";

                    case 1:
                        return "Прибор";

                    case 2:
                        return "Датчик";

                    case 3:
                        return "Исполнительное устройство";

                    case 4:
                        return "Сеть передачи данных";

                    case 5:
                        return "Удаленный сервер";

                    case 6:
                        return "[Без устройства]";

                    default:
                        return "";
                }
            }
        }
    }
}
