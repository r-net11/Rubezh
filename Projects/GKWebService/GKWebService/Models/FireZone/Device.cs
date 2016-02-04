using System;

namespace GKWebService.Models
{
    public class Device
    {
		/// <summary>
		/// Идентификатор устройства
		/// </summary>
		public Guid Uid { get; set; }
		
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
		public String Description { get; set; }

        public Int32 Level { get; set; }

        /// <summary>
        /// Индикатор статуса
        /// </summary>
        public String StateIcon { get; set; }

        /// <summary>
        /// Логотип устройства
        /// </summary>
        public String ImageDeviceIcon { get; set; }
    }
}