using System;

namespace GKWebService.Models.FireZone
{
    public class Device
    {
        /// <summary>
        /// Наименование устройства
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Адрес устройства
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public String Note { get; set; }

        public Int32 Level { get; set; }

        /// <summary>
        /// Индикатор статуса
        /// </summary>
        public String StateIcon { get; set; }

        /// <summary>
        /// Логотип устройства
        /// </summary>
        public Tuple<string, System.Drawing.Size> ImageDeviceIcon { get; set; }
    }
}