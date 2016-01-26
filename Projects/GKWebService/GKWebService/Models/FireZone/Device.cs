using GKWebService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using RubezhAPI.GK;

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
        public Tuple<string, System.Drawing.Size> StateImageSource { get; set; }

        /// <summary>
        /// Логотип устройства
        /// </summary>
        public Tuple<string, System.Drawing.Size> ImageBloom { get; set; }
    }
}