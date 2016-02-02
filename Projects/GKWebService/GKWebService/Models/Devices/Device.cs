using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models.Devices
{
    public class Device
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid UID { get; set; }

        /// <summary>
        /// Идентификатор родителя
        /// </summary>
        public Guid? ParentUID { get; set; }

        /// <summary>
        /// Наименование устройства
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Адрес устройства
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// Уровень
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Является листом
        /// </summary>
        public bool IsLeaf { get; set; }

        /// <summary>
        /// Индикатор статуса
        /// </summary>
        public Tuple<string, System.Drawing.Size> StateImageSource { get; set; }

        /// <summary>
        /// Логотип устройства
        /// </summary>
        public Tuple<string, System.Drawing.Size> ImageBloom { get; set; }
    }
}